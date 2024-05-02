using ConsoleGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame.Scenes
{
    public class DungeonScene
    {
        private Character player;
        private Random random;
        private Dungeon dungeon;
        public DungeonScene(Character character)
        {
            player = character;
            random = new Random();
        }

        public void EnterDungeon()
        {
            Console.WriteLine("1. ���� ����     | ���� 5 �̻� ����");
            Console.WriteLine("2. �Ϲ� ����     | ���� 11 �̻� ����");
            Console.WriteLine("3. ����� ����    | ���� 17 �̻� ����");
            Console.WriteLine("0. ������");
            Console.Write("���Ͻô� �ൿ�� �Է����ּ���.\n>> ");

            int InputKey = Game.instance.inputManager.GetValidSelectedIndex((int)Difficulty.Max, (int)Difficulty.Easy);
            dungeon = new Dungeon((Difficulty)InputKey);

            if (!player.HasRequiredDefense(dungeon.requiredDefense))
            {
                Console.WriteLine($"������ {dungeon.requiredDefense} �̻��̾�� {dungeon.difficulty} ������ ������ �� �ֽ��ϴ�.");
                return;
            }

            Start(dungeon.difficulty);

            DropNormalItem(dungeon.difficulty);
            DropSpecialItem(dungeon.difficulty);

        }

        private int CalculateAdditionalReward(int attackPower)
        {
            double percentage = random.Next(10, 21) / 100.0; // 10% ~ 20% ���� ��

            int additionalReward = (int)(attackPower * percentage);  // double ���� int�� ĳ����

            return additionalReward;
        }

        private int CalculateReward(int attackPower)
        {
            int additionalReward = CalculateAdditionalReward(attackPower);

            int totalReward = dungeon.baseReward + additionalReward;

            return totalReward;
        }

        public void Start(Difficulty difficulty)
        {
            if (!player.HasRequiredDefense(dungeon.requiredDefense))
            {
                Console.WriteLine($"������ {dungeon.requiredDefense} �̻��̾�� {difficulty} ������ ������ �� �ֽ��ϴ�.");
                return;
            }

            bool success = random.Next(1, 101) <= 60;

            if (!success)
            {
                Console.WriteLine($"{difficulty} ���� ���� ����! ������ óġ�ϸ� ������ ������ ü���� �������� �����մϴ�.");
                player.Health /= 2;
                return;
            }

            Console.WriteLine($"{difficulty} ���� ���� ����!");

            Enemy enemy = GenerateEnemy(difficulty);

            Console.WriteLine($"[�� ����: {enemy.Name}, ���� {enemy.Level}, ü�� {enemy.Health}, ���ݷ� {enemy.AttackPower}]");

            while (player.Health > 0 && enemy.Health > 0)
            {
                Console.WriteLine("\n���� �����ϼ���:");
                Console.WriteLine("1. ����");
                Console.WriteLine("2. ��ų");
                Console.WriteLine("3. ������ ���");
                Console.Write(">> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        player.Attack(enemy);
                        break;
                    case "2":
                        UseCharacterSkill(player, enemy);
                        break;
                    case "3":
                        UseItem();
                        break;
                    default:
                        Console.WriteLine("�߸��� �����Դϴ�.");
                        break;
                }

                if (enemy.Health > 0)
                {
                    enemy.EnemyAttack(player);
                }
            }

            if (player.Health <= 0)
            {
                Console.WriteLine("����� �й��߽��ϴ�.");
            }
            else
            {
                Console.WriteLine("���� �����ƽ��ϴ�!");
                int additionalReward = CalculateAdditionalReward(player.AttackPower);

                Console.WriteLine($"�⺻ ����: {dungeon.baseReward} G");
                Console.WriteLine($"���ݷ����� ���� �߰� ����: {additionalReward} G");

                int totalReward = dungeon.baseReward + additionalReward;
                Console.WriteLine($"�� ����: {totalReward} G�� ȹ���Ͽ����ϴ�.");

                player.Gold += totalReward;

                if (random.Next(1, 101) <= 20) // 15~20% Ȯ���� ���
                {


                    // DropHighTierItem();

                }
            }

            ClearDungeon();
        }


        private void UseCharacterSkill(Character player, Enemy enemy)
        {
            // ��ų ��� �޼��� ȣ��
            player.UseSkill(enemy);
        }

        private void ClearDungeon()
        {
            int damage = CalculateDamage();

            player.Health -= damage;

            Console.WriteLine($"���� Ŭ����! ü�� {damage} �Ҹ��.");
            Console.WriteLine($"���� ü��: {player.Health}");


            player.Exp += 1;       // ���� ����ĥ ������ ����ġ 1 ����
            Console.WriteLine($"����ġȹ��: {player.Exp}");

            player.LevelUp.CheckLevelUp();


            if (random.Next(1, 101) <= 20) // 20% Ȯ���� Ư���� ������ ���
            {
                DropSpecialItem(dungeon.difficulty); // difficulty�� ����
            }

            // ����� �Է� ��ٸ���
            Console.ReadLine();
        }


        private void DropNormalItem(Difficulty difficulty)
        {
            // ���� �������� �⺻ ������
            if (difficulty == Difficulty.Easy)
            {
                // ������ ī�װ����� ������
                var armorItems = Game.instance.itemManager.ItemInfos.Where(item => item.Type == ItemType.Armor).ToList();
                var weaponItems = Game.instance.itemManager.ItemInfos.Where(item => item.Type == ItemType.Weapon).ToList();
                var consumableItems = Game.instance.itemManager.ItemInfos.Where(item => item.Type == ItemType.Consumable).ToList();

                // �������� ���� �Ǵ� ���� ����
                Item droppedItem = null;
                if (random.Next(4) == 0) // 0 �Ǵ� 1�� �����ϰ� ��ȯ�ϹǷ� 25% Ȯ���� ������ ����, ���� ��, �Һ� ������ �� �� �ϳ��� ���
                {
                    droppedItem = armorItems[random.Next()];
                }
                else
                {
                    droppedItem = weaponItems[random.Next()];
                }

                // �������� ���� ����
                Item consumableItem = consumableItems[random.Next(consumableItems.Count)];

                // �������� ���õ� ������ ���
                Console.WriteLine($"��� �������� ȹ���Ͽ����ϴ�: {droppedItem}");
                Console.WriteLine($"�Һ� �������� ȹ���Ͽ����ϴ�: {consumableItem}");


                // �������� �κ��丮�� ��� ī�װ��� �߰�
                player.InventoryManager.AddItem(droppedItem);
                player.InventoryManager.AddItem(consumableItem);
            }

        }

        private void DropSpecialItem(Difficulty difficulty)
        {
            // �븻 �������� �ϵ� �������� Ư�� ������ ���
            if (difficulty == Difficulty.Normal ||
                difficulty == Difficulty.Hard)
            {
                // �������� �ϳ��� ������ ����
                Item droppedItem = Game.instance.itemManager.specialItems[random.Next(Game.instance.itemManager.specialItems.Count)];

                Console.WriteLine($"Ư���� �������� ȹ���Ͽ����ϴ�: {droppedItem.Name}");

                // �ͼ� �������̹Ƿ� Purchased ���� true�� ����
                Game.instance.itemManager.UpdateItemPurchasedStatus(droppedItem);

                // �������� �κ��丮�� ��� ī�װ��� �߰�
                player.InventoryManager.AddItem(droppedItem);
            }
        }



        private int CalculateDamage()
        {
            int baseDamage = random.Next(20, 36); // 20 ~ 35 ���� ��
            int difference = player.DefensePower - dungeon.requiredDefense;
            int extraDamage = difference > 0 ? random.Next(difference + 1) : 0;
            int totalDamage = baseDamage + extraDamage;

            return totalDamage;
        }

        //���� �۾��ϴ� ���� �������� �ҵ�
        private Enemy GenerateEnemy(Difficulty difficulty)
        {
            int level;
            int health;
            int attackPower;

            switch (difficulty)
            {
                case Difficulty.Easy:
                    level = player.Level + 1;
                    health = 50 + (level * 10);
                    attackPower = 5 + (level * 2);
                    break;
                case Difficulty.Normal:
                    level = player.Level + 2;
                    health = 200 + (level * 20);
                    attackPower = 20 + (level * 10);
                    break;
                case Difficulty.Hard:
                    level = player.Level + 5;
                    health = 350 + (level * 40);
                    attackPower = 35 + (level * 40);
                    break;
                default:
                    throw new ArgumentException("Invalid difficulty");
            }

            return new Enemy(level, health, attackPower, $"�� ���� {level}");
        }

        private void UseItem()
        {
            Console.WriteLine("����� �������� �����ϼ���.");
            // ������ ��� ������ �������� ���߽��ϴ�
        }
    }
}
