using System.Collections.Generic;

namespace ConsoleGame.Managers
{
    public class ItemManager
    {
        int UniqueKey = 1;
        public List<Item> ItemInfos = new List<Item>();
        public List<Item> specialItems = new List<Item>();
        public Dictionary<e_ItemStatusType, int?> dicStatusBonus { get; set; } = new Dictionary<e_ItemStatusType, int?>();
        public void InitItemList()
        {
            ItemInfos.Add(new Item(UniqueKey++, "조개 껍데기", ItemType.Weapon, 1000, SetItemStatus(e_ItemStatusType.Attack, 5), "물개 아저씨의 비상식량이었습니다. 쓸만합니다."));
            ItemInfos.Add(new Item(UniqueKey++, "말린 연어", ItemType.Weapon, 1500, SetItemStatus(e_ItemStatusType.Attack, 15), "연어를 햇볕에 말려 딱딱해졌습니다. 제법 단단합니다."));
            ItemInfos.Add(new Item(UniqueKey++, "참치 대가리", ItemType.Weapon, 2500, SetItemStatus(e_ItemStatusType.Attack, 25), "어린 참치의 동족으로 만들어진 검입니다. 묵직합니다!"));
            ItemInfos.Add(new Item(UniqueKey++, "냉동 상어", ItemType.Weapon, 3500, SetItemStatus(e_ItemStatusType.Attack, 35), "영하 30도의 온도로 꽝꽝 얼려진 냉동 대검입니다!"));
            ItemInfos.Add(new Item(UniqueKey++, "펭귄 성장용 깃털", ItemType.Armor, 900, SetItemStatus(e_ItemStatusType.Defense, 5), "특별 제작된 펭모로 늠름함을 높입니다."));
            ItemInfos.Add(new Item(UniqueKey++, "북극곰의 투구", ItemType.Armor, 1200, SetItemStatus(e_ItemStatusType.Defense, 10), "털갈이를 하는 북극곰의 옆에서 채취해 만든 투구입니다."));
            ItemInfos.Add(new Item(UniqueKey++, "순록의 털옷", ItemType.Armor, 2100, SetItemStatus(e_ItemStatusType.Defense, 15), "북극에서 천적에 의해 사냥당한 순록의 털로 만든 옷입니다."));
            ItemInfos.Add(new Item(UniqueKey++, "방문객의 연구복", ItemType.Armor, 3000, SetItemStatus(e_ItemStatusType.Defense, 20), "이곳을 연구하는 연구원이 마지막으로 남기고 간 옷입니다."));
            ItemInfos.Add(new Item(UniqueKey++, "새우", ItemType.Consumable, 200, SetItemStatus(e_ItemStatusType.RecoveryHp, 20), "맛있습니다! 체력을 20 회복하는 음식입니다."));
            ItemInfos.Add(new Item(UniqueKey++, "조개살 죽", ItemType.Consumable, 200, SetItemStatus(e_ItemStatusType.RecoveryMp, 20), "물개 아저씨의 야심작입니다! 마나를 20 회복하는 음식입니다."));
            ItemInfos.Add(new Item(UniqueKey++, "해파리", ItemType.Consumable, 1000, SetItemStatus(e_ItemStatusType.AdditionalDamage, 5), "먹으면 따끔합니다! 공격력이 5 증가하는 음식입니다."));

            specialItems.Add(new Item(UniqueKey++, "늑대 클로", ItemType.Weapon, 9000, SetItemStatus(e_ItemStatusType.Attack, 35), "늑대의 발톱으로 만들었습니다. 도적이 되고 싶은 자, 여기로...", true));
            specialItems.Add(new Item(UniqueKey++, "범고래의 꼬리뼈", ItemType.Weapon, 9500, SetItemStatus(e_ItemStatusType.Attack, 38), "날카롭고 아픕니다! 펭귄은 이제 썰어버릴 수 있습니다.", true));
            specialItems.Add(new Item(UniqueKey++, "바다코끼리의 엄니", ItemType.Weapon, 10000, SetItemStatus(e_ItemStatusType.Attack, 40), "범고래에게는 깐부였던 것, 이제 누가 최강이지?", true));
            specialItems.Add(new Item(UniqueKey++, "도둑갈매기의 깃털", ItemType.Armor, 11000, SetItemStatus(e_ItemStatusType.Defense, 45), "장착해도 펭귄은 날 수 없습니다, 꿈 깨세요!", true));
            specialItems.Add(new Item(UniqueKey++, "오리발", ItemType.Armor, 11500, SetItemStatus(e_ItemStatusType.Defense, 48), "바다에서 주웠습니다! 누구보다 빠르게 남들과는 다르게 물속을 누빌 수 있습니다!", true));
            specialItems.Add(new Item(UniqueKey++, "구미호의 팔찌", ItemType.Armor, 12000, SetItemStatus(e_ItemStatusType.Defense, 50), "희귀하다는 구미호의 꼬리로 만든 팔찌입니다! 착용시 늑대에게 물려도 정신차릴 수 있습니다!", true));
            specialItems.Add(new Item(UniqueKey++, "북극곰의 수트", ItemType.Armor, 13000, SetItemStatus(e_ItemStatusType.Defense, 55), "최고급 곰털로 만들었습니다. 착용시 범고래에게 두번이나 물려도 삽니다!", true));
            specialItems.Add(new Item(UniqueKey++, "북극곰의 간", ItemType.Consumable, 18000, SetItemStatus(e_ItemStatusType.MaxHealth, 30), "별미라고 소문났습니다! 사용시 북극곰을 찢을 수 있게 됩니다!", true));
        }


        public Dictionary<e_ItemStatusType, int> SetItemStatus(e_ItemStatusType type, int value)
        {
            Dictionary<e_ItemStatusType, int> temp = new Dictionary<e_ItemStatusType, int>();
            temp.Add(type, value);
            return temp;
        }

        public void UpdateItemPurchasedStatus(Item item)
        {
            foreach (var itemInfo in ItemInfos)
            {
                if (itemInfo.Name == item.Name && itemInfo.Type == item.Type)
                {
                    itemInfo.Purchased = true;
                    break;
                }
            }
        }
    }
}