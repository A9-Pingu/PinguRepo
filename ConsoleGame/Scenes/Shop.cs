using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleGame.Managers;

namespace ConsoleGame.Scenes
{
    public class Shop
    {
        Character player;
        ItemManager ItemManager;
        public Shop(Character character, ItemManager itemManager)
        {
            player = character;
            ItemManager = itemManager;
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
                        ShowItemActions(ItemType.Consumable);
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
                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 나가기");
                Console.Write("원하시는 행동을 선택해주세요.\n>> ");

                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(2);

                switch (inputKey)
                {
                    case 1:
                        BulkBuying(itemType);
                        break;
                    case 2:
                        SellItem();
                        break;
                    case 0:
                        Console.WriteLine("상점에서 나갑니다");
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        break;
                }
                Thread.Sleep(1000);
            }

        }

        public void BulkBuying(ItemType itemType)
        {
            Game.instance.uiManager.ShowItemsForSale(itemType);
            Console.WriteLine("구매할 아이템 번호를 선택해주세요. (취소: 0)");
            Console.Write(">>");
            var itemsToBuy = ItemManager.ItemInfos.Where(i => i.Type == itemType).ToList();

            int choice = Game.instance.inputManager.GetValidSelectedIndex(itemsToBuy.Count);

            if (choice == 0)
            {
                Console.WriteLine("구매가 취소되었습니다.");
                return;
            }

            var selectedToBuy = itemsToBuy[choice - 1];

            Console.WriteLine($"\n구매할 {selectedToBuy.Name}의 수량을 입력하세요: ");
            Console.Write(">>");

            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                int totalPrice = selectedToBuy.Price * quantity;

                if (player.Gold >= totalPrice)
                {
                    player.Gold -= totalPrice;
                    player.InventoryManager.AddItem(selectedToBuy, quantity);
                    if (itemType == ItemType.Weapon || itemType == ItemType.Armor)
                    {
                        ItemManager.UpdateItemPurchasedStatus(selectedToBuy);
                    }
                    Console.WriteLine($"\n{selectedToBuy.Name} {quantity}개를 구매했습니다.");
                }
                else
                {
                    Console.WriteLine("\n골드가 부족합니다.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("\n잘못된 수량입니다.");
                return ;
            }

            Thread.Sleep(1000);
        }

        private void SellItem()
        {
            var inventoryManager = player.InventoryManager;

            if (inventoryManager.dicInventory.Count == 0)
            {
                Console.WriteLine("인벤토리가 비어 있습니다.");
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
                return;
            }

            index = inventoryManager.GetItemKey(inputKey);

            //개수
            BulkSelling(inventoryManager.dicInventory[index]);
            Thread.Sleep(1000);
        }

        public void BulkSelling(Item item)
        {
            Console.Write($"판매할 {item.Name}의 수량을 입력하세요: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && item.Count >= quantity && quantity > 0)
            {
                if (item.Type == ItemType.Weapon || item.Type == ItemType.Armor)
                {
                    bool isContinue = ReleasedAndSellingEquipment(item.UniqueKey, quantity);
                    if(!isContinue) 
                    {
                        return;
                    }
                }

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

        //호출 시 -1해서 매개변수넣을것?
        public bool ReleasedAndSellingEquipment(int index, int count)
        {
            if (player.InventoryManager.dicInventory[index].Equipped && player.InventoryManager.dicInventory[index].Count - count == 0)
            {
                Console.WriteLine($"{player.InventoryManager.dicInventory[index].Name}은(는) 현재 장착 중입니다. 장착을 해제하고 판매하시겠습니까? (Y/N)");
                string confirm = Console.ReadLine().ToUpper();

                if (confirm != "Y")
                {
                    Console.WriteLine("판매가 취소되었습니다.");
                    return false;
                }
                player.InventoryManager.EquipItem(player.InventoryManager.dicInventory[index]);
            }

            return true;
        }
    }
}