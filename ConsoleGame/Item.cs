namespace ConsoleGame
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        All
    }
    //
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
        public int UniqueKey { get; private set; }
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public Dictionary<e_ItemStatusType, int> dicStatusBonus { get; set; } = new Dictionary<e_ItemStatusType, int>();
        public string Description { get; set; }
        public int Price { get; set; }
        public bool Purchased { get; set; }
        public bool Equipped { get; set; } = false;
        public bool IsBound { get; set; }  // 귀속 아이템 여부 추가
        public int Count { get; set; } = 0;


        // 귀속 아이템을 생성할 때는 IsBound 값을 true로 설정합니다
        public Item(int keyValue, string name, ItemType type, int price, Dictionary<e_ItemStatusType, int> stat, string description, bool isBound = false, bool purchased = false)
        {
            UniqueKey = keyValue;
            Name = name;
            Type = type;
            Price = price;
            Equipped = false;
            dicStatusBonus = stat;
            Purchased = purchased;
            Description = description;
            IsBound = isBound; // 아이템이 귀속 되었는지 여부를 표시
        }
    }
}