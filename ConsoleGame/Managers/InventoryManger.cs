using System;
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

        public InventoryManager()
        {
            dicEquipItem[ItemType.Weapon] = null;
            dicEquipItem[ItemType.Armor] = null;
            dicEquipItem[ItemType.Consumable] = null;
            dicEquipItem[ItemType.All] = null;
        }
        // 인벤토리 초기화
        public void Init(Character data)
        {
            player = Game.instance.player;
            dicEquipItem = data.InventoryManager.dicEquipItem;
            dicInventory = data.InventoryManager.dicInventory;
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

        public bool CheckedEquipItem(Item item)
        {
            if (dicEquipItem[item.Type].UniqueKey == item.UniqueKey)
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


            if (dicEquipItem[item.Type] == null)
            {
                dicEquipItem[item.Type] = item;
                item.Equipped = true;
                AddItemStatBonus(item);
                Game.instance.questManager.dicQuestInfos[2].OnCheckEvent(2, 1);
                Console.WriteLine("\t[아이템]{0}을/를 장착하였습니다.", item.Name);
            }
            else if (CheckedEquipItem(item))
            {
                RemoveItemStatBonus(item);
                item.Equipped = false;
                dicEquipItem[item.Type] = null;
                Console.WriteLine("\t[아이템]{0}을/를 장착해제하였습니다.", item.Name);
            }
            else
            {
                RemoveItemStatBonus(dicEquipItem[item.Type]);
                foreach (var item2 in dicInventory) 
                { 
                    if(item2.Value.Count == dicEquipItem[item.Type].Count)
                        item2.Value.Equipped = false;
                }
                dicEquipItem[item.Type].Equipped = false;
                Console.WriteLine("\t[아이템]{0}을/를 장착해제하였습니다.", dicEquipItem[item.Type].Name);
                dicEquipItem[item.Type] = item;
                item.Equipped = true;
                AddItemStatBonus(item);
                Console.WriteLine("\t[아이템]{0}을/를 장착하였습니다.", item.Name);
                Game.instance.questManager.dicQuestInfos[2].OnCheckEvent(2, 1);

            }
            Game.instance.inputManager.InputAnyKey();
        }

        public int StatBonus(Item item, int nStat, e_ItemStatusType type, bool isIncrease, string strStat)
        {
            Console.Write($"캐릭터의 {strStat}이 {nStat} 에서");
            if (isIncrease)
            {
                nStat += (int)item.dicStatusBonus[type];
                Console.WriteLine("{0} 만큼 증가하였습니다", (int)item.dicStatusBonus[type]);
            }
            else
            {
                nStat -= (int)item.dicStatusBonus[type];
                Console.WriteLine("{0} 만큼 감소하였습니다", (int)item.dicStatusBonus[type]);
            }
            Console.WriteLine($"현재 {strStat} : {nStat}");
            return nStat;
        }
        public void AddItemStatBonus(Item item)
        {
            foreach (var key in item.dicStatusBonus.Keys)
            {
                switch (key)
                {
                    case e_ItemStatusType.Attack:
                        player.AttackPower = StatBonus(item, player.AttackPower, e_ItemStatusType.Attack, true, "공격력");
                        break;

                    case e_ItemStatusType.Defense:
                        player.DefensePower = StatBonus(item, player.DefensePower, e_ItemStatusType.Defense, true, "방어력");
                        break;

                    case e_ItemStatusType.MaxHealth:
                        player.MaxHealth = StatBonus(item, player.MaxHealth, e_ItemStatusType.MaxHealth, true, "최대 체력");
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
                        player.MP = StatBonus(item, player.MP, e_ItemStatusType.RecoveryMp, true, "마나");
                        break;

                    case e_ItemStatusType.AdditionalDamage:
                        player.AdditionalDamage = StatBonus(item, player.AdditionalDamage, e_ItemStatusType.AdditionalDamage, true, "추가 데미지");
                        break;
                }
            }
            Game.instance.questManager.dicQuestInfos[3].OnCheckEvent(3, player.AttackPower, player.DefensePower);
        }

        public void RemoveItemStatBonus(Item item)
        {
            foreach (var key in item.dicStatusBonus.Keys)
            {
                switch (key)
                {
                    case e_ItemStatusType.Attack:
                        player.AttackPower = StatBonus(item, player.AttackPower, e_ItemStatusType.Attack, false, "공격력");
                        break;

                    case e_ItemStatusType.Defense:
                        player.DefensePower = StatBonus(item, player.DefensePower, e_ItemStatusType.Defense, false, "방어력");
                        break;

                    case e_ItemStatusType.MaxHealth:
                        player.MaxHealth = StatBonus(item, player.MaxHealth, e_ItemStatusType.MaxHealth, false, "최대 체력");
                        break;

                    case e_ItemStatusType.RecoveryHp:
                        player.Health = StatBonus(item, player.Health, e_ItemStatusType.RecoveryHp, false, "체력");
                        break;

                    case e_ItemStatusType.RecoveryMp:
                        player.MP = StatBonus(item, player.MP, e_ItemStatusType.RecoveryMp, false, "마나");
                        break;

                    case e_ItemStatusType.AdditionalDamage:
                        player.AdditionalDamage = StatBonus(item, player.AdditionalDamage, e_ItemStatusType.AdditionalDamage, true, "추가 데미지");
                        break;
                }
            }
        }
    }
}
