ausing System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleGame.Managers
{
    public class InventoryManager
    {

        public Dictionary<ItemType, Item> dicEquipItem = new Dictionary<ItemType, Item>();
        public Character player;
        public Dictionary<int, Item> dicInventory = new Dictionary<int, Item>();

        // 인벤토리 초기화
        public InventoryManager(Character character)
        {
            player = character;
            dicEquipItem[ItemType.Weapon] = null;
            dicEquipItem[ItemType.Armor] = null;
            dicEquipItem[ItemType.Consumable] = null;
            dicEquipItem[ItemType.All] = null;
        }

        // 인벤토리 출력 및 아이템 장착/해제 기능
        public void ShowInventory()
        {
            while (true)
            {
                bool isEmpty = Game.instance.uiManager.DisplayInventory(this);
                if (!isEmpty)
                {
                    Game.instance.inputManager.InputAnyKey();
                    return;
                }

                int actionIndex = Game.instance.inputManager.GetValidSelectedIndex(1);

                switch (actionIndex)
                {
                    case 0:
                        Console.WriteLine("인벤토리에서 나갑니다.");
                        Game.instance.inputManager.InputAnyKey();
                        return;
                    case 1:
                        ManagedEquip();
                        break;
                    default:
                        break;
                }
                Thread.Sleep(1000);
            }
        }

        // 아이템 타입에 따른 아이템 목록 반환
        public Item GetItemsByType(ItemType itemType)
        {
            return dicEquipItem[itemType];
        }

        // 아이템 추가
        public void AddItem(Item item, int count = 1)
        {
            if (!dicInventory.ContainsKey(item.UniqueKey))
            {
                dicInventory[item.UniqueKey] = item;
                dicInventory[item.UniqueKey].Purchased = true;
            }
            dicInventory[item.UniqueKey].Count += count;
        }


        // 아이템 삭제 //else는 안뜨는게 정상...만약 뜨면 신고바랍니다.
        public bool RemoveItem(Item item, int count = 1)
        {
            if (dicInventory.ContainsKey(item.UniqueKey))
            {
                if (dicInventory[item.UniqueKey].Count - count > 0)
                {
                    dicInventory[item.UniqueKey].Count -= count;
                }
                else if (dicInventory[item.UniqueKey].Count - count == 0)
                {
                    dicInventory[item.UniqueKey].Count -= count;
                    dicInventory[item.UniqueKey].Purchased = false;
                    dicInventory.Remove(item.UniqueKey);
                }
                else
                {
                    Console.WriteLine("충분한 개수가 없습니다.");
                    return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("아이템이 인벤토리에 없습니다.");
                return false;
            }
        }

        // 인덱스로 아이템 조회
        public Item GetItem(int index)
        {
            return dicInventory[index];
        }

        private void ManagedEquip()
        {
            Console.Write("아이템 번호를 입력하세요: ");
            int inputKey = Game.instance.inputManager.GetValidSelectedIndex(dicInventory.Count);
            if (inputKey == 0)
            {
                return;
            }

            int index = GetItemKey(inputKey);
            if (dicInventory[index].Type == ItemType.Consumable || dicInventory[index].Type == ItemType.All)
            {
                AddItemStatBonus(dicInventory[index]);
                RemoveItem(dicInventory[index]);
            }
            else
            {
                EquipItem(dicInventory[index]);
            }
        }

        public int GetItemKey(int inputKey)
        {
            int index = 0;
            foreach (var item in dicInventory)
            {
                index++;
                if (inputKey == index)
                {
                    index = item.Value.UniqueKey;
                    return index;
                }
            }
            Console.WriteLine("에러 발생");
            return 0;
        }
        public string GetCategoryName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                case ItemType.Armor:
                    return "장비";
                case ItemType.Consumable:
                    return "소비";
                case ItemType.All:
                    return "기타";
                default:
                    return "알 수 없음";
            }
        }

        public bool CheckedEquipItem(Item item)
        {
            if (dicEquipItem[item.Type] == item)
            {
                return true;
            }
            return false;
        }

        public void EquipItem(Item item)
        {
            if (item.Type == ItemType.Consumable || item.Type == ItemType.All)
            {
                Console.WriteLine("장착 아이템이 아닙니다.");
                Thread.Sleep(1000);
                return;
            }

            if (CheckedEquipItem(item))
            {
                RemoveItemStatBonus(item);
                item.Equipped = false;
                dicEquipItem[item.Type] = null;
            }
            else
            {
                if (dicEquipItem[item.Type] != null)
                {
                    RemoveItemStatBonus(dicEquipItem[item.Type]);
                    dicEquipItem[item.Type].Equipped = false;
                }
                dicEquipItem[item.Type] = item;
                item.Equipped = true;
                Game.instance.questManager.dicQuestInfos[1].OnCheckEvent(1, 1);
                AddItemStatBonus(item);
            }
            Console.WriteLine("\t[아이템]{0}을/를 장착{1}하였습니다.", item.Name, CheckedEquipItem(item) ? " " : " 해제");
            Thread.Sleep(1000);
        }


        public void AddItemStatBonus(Item item)
        {
            foreach (var key in item.dicStatusBonus.Keys)
            {
                switch (key)
                {
                    case e_ItemStatusType.Attack:
                        Console.Write($"캐릭터의 공격력이 {player.AttackPower} 에서");
                        player.AttackPower += (int)item.dicStatusBonus[e_ItemStatusType.Attack];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.Attack]} 만큼 증가하였습니다");
                        Console.WriteLine($"현재 공격력 : {player.AttackPower}");
                        break;

                    case e_ItemStatusType.Defense:
                        Console.Write($"캐릭터의 방어력이 {player.DefensePower} 에서");
                        player.DefensePower += (int)item.dicStatusBonus[e_ItemStatusType.Defense];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.Defense]} 만큼 증가하였습니다");
                        Console.WriteLine($"현재 방어력 : {player.DefensePower}");
                        break;

                    case e_ItemStatusType.MaxHealth:
                        Console.Write($"캐릭터의 최대 체력이 {player.MaxHealth} 에서");
                        player.MaxHealth += (int)item.dicStatusBonus[e_ItemStatusType.MaxHealth];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.MaxHealth]} 만큼 증가하였습니다");
                        Console.WriteLine($"현재 최대 체력 : {player.MaxHealth}");
                        break;

                    case e_ItemStatusType.RecoveryHp:
                        Console.Write($"캐릭터의 체력을 {player.Health} 에서 ");
                        if ((player.Health + (int)item.dicStatusBonus[e_ItemStatusType.RecoveryHp]) < player.MaxHealth)
                        {
                            player.Health += (int)item.dicStatusBonus[e_ItemStatusType.RecoveryHp];
                            Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.RecoveryHp]} 만큼 회복하였습니다");
                        }
                        else
                        {
                            Console.WriteLine($"{player.MaxHealth - player.Health} 만큼 회복하였습니다");
                            player.Health = player.MaxHealth;
                        }
                        Console.WriteLine($"현재 체력 : {player.Health}");
                        break;

                    case e_ItemStatusType.RecoveryMp:
                        Console.Write($"캐릭터의 마나를 {player.MP} 에서 ");
                        player.MP += (int)item.dicStatusBonus[e_ItemStatusType.RecoveryMp];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.RecoveryMp]} 만큼 회복하였습니다");
                        Console.WriteLine($"현재 마나 : {player.MP}");
                        break;

                    case e_ItemStatusType.AdditionalDamage:
                        Console.Write($"캐릭터의 추가 데미지가 {player.AdditionalDamage} 에서 ");
                        player.AdditionalDamage += (int)item.dicStatusBonus[e_ItemStatusType.AdditionalDamage];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.AdditionalDamage]} 만큼 증가하였습니다");
                        Console.WriteLine($"현재 추가 데미지 : {player.AdditionalDamage}");
                        break;
                }
            }
            Game.instance.questManager.dicQuestInfos[2].OnCheckEvent(2, player.AttackPower, player.DefensePower);
        }

        public void RemoveItemStatBonus(Item item)
        {
            foreach (var key in item.dicStatusBonus.Keys)
            {
                switch (key)
                {
                    case e_ItemStatusType.Attack:
                        Console.Write($"캐릭터의 공격력이 {player.AttackPower} 에서");
                        player.AttackPower -= (int)item.dicStatusBonus[e_ItemStatusType.Attack];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.Attack]} 만큼 감소하였습니다");
                        Console.WriteLine($"현재 공격력 : {player.AttackPower}");
                        break;
                    case e_ItemStatusType.Defense:
                        Console.Write($"캐릭터의 방어력이 {player.DefensePower} 에서");
                        player.DefensePower -= (int)item.dicStatusBonus[e_ItemStatusType.Defense];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.Defense]} 만큼 감소하였습니다");
                        Console.WriteLine($"현재 방어력 : {player.DefensePower}");
                        break;
                    case e_ItemStatusType.MaxHealth:
                        Console.Write($"캐릭터의 최대 체력이 {player.MaxHealth} 에서");
                        player.MaxHealth -= (int)item.dicStatusBonus[e_ItemStatusType.MaxHealth];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.MaxHealth]} 만큼 감소하였습니다");
                        Console.WriteLine($"현재 최대 체력 : {player.MaxHealth}");
                        break;
                    case e_ItemStatusType.RecoveryHp:
                        Console.Write($"캐릭터의 체력을 {player.Health} 에서 ");
                        player.Health -= (int)item.dicStatusBonus[e_ItemStatusType.RecoveryHp];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.RecoveryHp]} 만큼 회복하였습니다");
                        Console.WriteLine($"현재 체력 : {player.Health}");
                        break;
                    case e_ItemStatusType.RecoveryMp:
                        Console.Write($"캐릭터의 마나를 {player.MP} 에서 ");
                        player.MP -= (int)item.dicStatusBonus[e_ItemStatusType.RecoveryMp];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.RecoveryMp]} 만큼 감소하였습니다");
                        Console.WriteLine($"현재 마나 : {player.MP}");
                        break;
                    case e_ItemStatusType.AdditionalDamage:
                        Console.Write($"캐릭터의 추가 데미지가 {player.AdditionalDamage} 에서 ");
                        player.AdditionalDamage -= (int)item.dicStatusBonus[e_ItemStatusType.AdditionalDamage];
                        Console.WriteLine($"{(int)item.dicStatusBonus[e_ItemStatusType.AdditionalDamage]} 만큼 감소하였습니다");
                        Console.WriteLine($"현재 추가 데미지 : {player.AdditionalDamage}");
                        break;
                }
            }
        }
    }
}