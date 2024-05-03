namespace ConsoleGame
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        All
    }
    public enum e_ItemStatusType
    {
        Attack = 0,
        Defense,
        MaxHealth,
        RecoveryHp,
        RecoveryMp,
        AdditionalDamage,
        MaxStatusType
    }

    public class Item
    {
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public int StatBonus { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public bool Purchased { get; set; }
        public bool Equipped { get; set; } = false;
        public bool IsBound { get; set; }  // 귀속 아이템 여부 추가
        public int Count { get; set; }


        // 귀속 아이템을 생성할 때는 IsBound 값을 true로 설정합니다
        public Item(string name, ItemType type, int price, int statBonus, string description, bool isBound = false, bool purchased = false)
        {
            Name = name;
            Type = type;
            Price = price;
            Equipped = false;
            StatBonus = statBonus;
            Purchased = purchased;
            Description = description;
            IsBound = isBound; // 아이템이 귀속 되었는지 여부를 표시
        }

        public override string ToString()
        {
            string purchaseStatus = Purchased ? "구매완료" : $"{Price} G";
            string stat = Type == ItemType.Armor ? $"방어력 +{StatBonus}" : $"공격력 +{StatBonus}";
            return $"{Name.PadRight(15)} | {stat.PadRight(10)} | {Description.PadRight(40)} | {purchaseStatus}";
        }
    }
}