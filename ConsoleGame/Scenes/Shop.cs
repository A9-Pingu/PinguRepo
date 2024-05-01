﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleGame.Managers;

namespace ConsoleGame.Scenes
{
    public class Shop
    {
        Character player;
        ItemManager item;
        public Shop(Character character, ItemManager itemManager)
        {
            player = character;
            item = itemManager;
        }
        public void ShowShop()
        {
            while (true)
            {
                Game.instance.uiManager.DisplayShopMenu();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowItemActions(ItemType.Weapon);
                        break;
                    case "2":                       
                        ShowItemActions(ItemType.Armor);
                        break;
                    case "3":
                        ShowConsumables();            // 소모품 목록 출력
                        BuyConsumable();      // 소모품 구매
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Game.instance.inputManager.InputAnyKey();
                        break;
                }
                Thread.Sleep(1000);
            }
        }


        private void ShowItemActions(ItemType itemType)
        {
            while (true)
            {
                Game.instance.uiManager.ShowItemsForSale(itemType);

                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(2);

                switch (inputKey)
                {
                    case 1:
                        BuyItem(itemType);
                        break;
                    case 2:
                        SellItem();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        break;
                }
            }

        }

        private void BuyItem(ItemType itemType)
        {
            Game.instance.uiManager.ShowItemsForSale(itemType);
            Console.WriteLine("구매할 아이템 번호를 선택해주세요. (취소: 0)");
            //
            var itemsToBuy = item.ItemInfos.Where(i => i.Type == itemType).ToList();

            int choice = Game.instance.inputManager.GetValidSelectedIndex(itemsToBuy.Count);

            if (choice == 0)
            {
                Console.WriteLine("구매가 취소되었습니다.");
                Thread.Sleep(1000);
                return;
            }

            var selectedToBuy = itemsToBuy[choice - 1];

            if (player.Gold < selectedToBuy.Price)
            {
                Console.WriteLine("골드가 부족합니다.");
                Thread.Sleep(1000);
                return;
            }

            player.Gold -= selectedToBuy.Price;
            item.UpdateItemPurchasedStatus(selectedToBuy);

            // 인벤토리에 아이템 추가
            player.InventoryManager.AddItem(selectedToBuy);

            Console.WriteLine($"{selectedToBuy.Name}을(를) 구매했습니다.");
            Thread.Sleep(1000);
        }


        private void SellItem()
        {
            var inventoryManager = player.InventoryManager;

            if (inventoryManager.dicInventory.Count == 0)
            {
                Console.WriteLine("인벤토리가 비어 있습니다.");
                Thread.Sleep(1000);
                return;
            }

            int index = 1;
            foreach (var item in inventoryManager.dicInventory)
            {
                Console.WriteLine($"{index++}. {item.Value.Name} ({item.Value.Type}) : {(int)(item.Value.Price * 0.85)} G  | * {item.Value.Count}");
            }

            Console.WriteLine("판매할 아이템을 선택하세요. (취소: 0)");

            int inputKey = Game.instance.inputManager.GetValidSelectedIndex(index);

            if (inputKey == 0)
            {
                Console.WriteLine("판매가 취소되었습니다.");
                Thread.Sleep(1000);
                return;
            }

            index = 0;
            foreach (var item in inventoryManager.dicInventory)
            {
                if (inputKey == index)
                    break;
                index++;
            }

            //개수
            BulkSelling(inventoryManager.dicInventory[index - 1]);
            Thread.Sleep(1000);
        }

        //호출 시 -1해서 매개변수넣을것
        public void ReleasedAndSellingEquipment(int index)
        {
            if (player.InventoryManager.dicInventory[index].Equipped && player.InventoryManager.dicInventory[index].Count == 1)
            {
                Console.WriteLine($"{player.InventoryManager.dicInventory[index].Name}은(는) 현재 장착 중입니다. 장착을 해제하고 판매하시겠습니까? (Y/N)");
                string confirm = Console.ReadLine().ToUpper();

                if (confirm != "Y")
                {
                    Console.WriteLine("판매가 취소되었습니다.");
                    Thread.Sleep(1000);
                    return;
                }
                player.InventoryManager.RemoveItemStatBonus(player.InventoryManager.dicInventory[index]);
            }
        }
        private void ShowConsumables()
        {
            Console.WriteLine("[소모품 목록]");

            int index = 1;
            foreach (var item in item.ItemInfos.Where(item => item.Type == ItemType.Consumable))
            {
                Console.WriteLine($"- {index++}. {item.Name} : {item.Price} G");
            }

            if (index == 1)
            {
                Console.WriteLine("소모품이 없습니다.");
            }
            Console.WriteLine();
        }

        private void BuyConsumable()
        {
            Console.Write("구매할 아이템 번호를 입력하세요: ");
            if (int.TryParse(Console.ReadLine(), out int itemIndex))
            {
                var consumables = item.ItemInfos.Where(item => item.Type == ItemType.Consumable).ToList();

                if (itemIndex >= 1 && itemIndex <= consumables.Count)
                {
                    var selectedConsumable = consumables[itemIndex - 1];
                    BulkBuying(selectedConsumable);
                }
                else
                {
                    Console.WriteLine("잘못된 아이템 번호입니다.");
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
            }
            Thread.Sleep(1000);
        }

        public void BulkBuying(Item item)
        {
            Console.Write($"구매할 {item.Name}의 수량을 입력하세요: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                int totalPrice = item.Price * quantity;

                if (player.Gold >= totalPrice)
                {
                    player.Gold -= totalPrice;
                    player.InventoryManager.AddItem(item, quantity);                    
                    Console.WriteLine($"{item.Name} {quantity}개를 구매했습니다.");
                }
                else
                {
                    Console.WriteLine("골드가 부족합니다.");
                }
            }
            else
            {
                Console.WriteLine("잘못된 수량입니다.");
            }
        }

        public void BulkSelling(Item item)
        {
            Console.Write($"판매할 {item.Name}의 수량을 입력하세요: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && item.Count >= quantity && quantity > 0)
            {
                if(item.Type == ItemType.Weapon || item.Type == ItemType.Armor)
                    ReleasedAndSellingEquipment(item.UniqueKey);
                int totalPrice = (int)(item.Price * quantity * 0.85);
                player.Gold += totalPrice;
                Console.WriteLine($"{item.Name} {quantity}개를 총 {totalPrice}G에 판매했습니다.");               
                player.InventoryManager.RemoveItem(item, quantity);
            }
            else
            {
                Console.WriteLine("잘못된 수량입니다.");
            }
        }
    }
}