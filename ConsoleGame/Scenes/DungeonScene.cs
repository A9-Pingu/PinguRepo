using ConsoleGame.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGame.Scenes
{
    public class DungeonScene
    {
        private Character player;
        private Random random;
        private Random random2 = new Random();
        public Character origin; ////////������ ���� ����
        private Dungeon dungeon;
        bool useItem = false;

        public DungeonScene(Character character)
        {
            player = character;
            origin = player.DeepCopy(); ////////������ ���� ����
            origin.Health = player.Health; ////////������ ���� ����
            Random random = new Random(Guid.NewGuid().GetHashCode());
        }

        //���� ���� ����
        public void EnterDungeon()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===================");
                Console.WriteLine("1. ���� ����     | ������ ����");
                Console.WriteLine("2. �Ϲ� ����     | ���� 30 �̻� ����");
                Console.WriteLine("3. ����� ����    | ���� 50 �̻� ����");
                Console.WriteLine("0. ������");
                Console.Write("���Ͻô� �ൿ�� �Է����ּ���.\n>> ");

                int InputKey = Game.instance.inputManager.GetValidSelectedIndex((int)Difficulty.Max - 1);
                dungeon = new Dungeon((Difficulty)InputKey);

                if (InputKey == 0)
                    return;

                else if (InputKey != 0 && dungeon.requiredDefense < player.DefensePower)
                {
                    Start(dungeon.difficulty); ;
                }
                else
                {
                    Console.WriteLine($"������ {dungeon.requiredDefense} �̻��̾�� {dungeon.difficulty} ������ ������ �� �ֽ��ϴ�.");
                    Game.instance.inputManager.InputAnyKey();
                    Console.Clear();
                }
            }
        }


        List<Enemy> deadMonsters = new List<Enemy>(); //���� ���� ��
        List<Enemy> selectedMonsters;
        public void Start(Difficulty difficulty)
        {
            player.OriginHealth = player.Health; //�÷��̾� �ʱ� ü��
            selectedMonsters = SelectMonsters(difficulty);

            Game.instance.uiManager.BattleScene(difficulty, selectedMonsters, player, false); //�ʱ�ȭ��
            Game.instance.inputManager.GetValidSelectedIndex(1, 1);
            while (true)
            {
                Game.instance.uiManager.BattleScene(difficulty, selectedMonsters, player, true);
                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(selectedMonsters.Count + 1);
                if (inputKey == 0)
                {
                    Console.WriteLine("===================");
                    Console.WriteLine("�̴�� ������ �����ðڽ��ϱ�?");
                    Console.WriteLine("1. ��");
                    Console.WriteLine("0. �ƴϿ�");
                    int nextKey = Game.instance.inputManager.GetValidSelectedIndex(1);
                    if (nextKey == 1)
                        return;
                    else
                        continue;
                }
                Battle(inputKey); //��Ʋ ����
                //UseItem();
                if (player.Health <= 0) //�÷��̾� ���
                {
                    LoseScene();
                    return;
                }
                else if (deadMonsters.Count == 3) //��� ���� ���
                {
                    ClearDungeon();
                    return;
                }
                //��ġ �ٲ�����
                UseItem();
                Game.instance.inputManager.InputAnyKey();
            }
        }
        private List<Enemy> SelectMonsters(Difficulty difficulty)
        {
            // ��� ����
            List<Enemy> allMonsters = new List<Enemy>
            {
                new Enemy("���ϰ��ű�", 2),
                new Enemy("���ϰ��ű�", 2),
                new Enemy("�߻��鰳", 2),
                new Enemy("�߻��鰳", 2),
                new Enemy("����", 2),
                new Enemy("����", 2),
                new Enemy("�ٴ�ǥ��", 3),
                new Enemy("�ٴ�ǥ��", 3),
                new Enemy("����", 3),
                new Enemy("����", 3),
                new Enemy("�ϱذ�", 4),
                new Enemy("�ϱذ�", 4),
                new Enemy("������", 5),
                new Enemy("������", 5),
            };

            //���̵��� ���� ���� 3���� ����
            int difficultyIndex = difficulty switch
            {
                Difficulty.Easy => 0,
                Difficulty.Normal => 4,
                Difficulty.Hard => 8,
                _ => 0
            };

            List<Enemy> selectedMonsters1 = new List<Enemy>();

            for (int i = difficultyIndex; i < difficultyIndex + 6; i++)
            {
                selectedMonsters1.Add(allMonsters[i]);
                Random random = new Random();
                if (selectedMonsters1.Count > 5)
                {
                    selectedMonsters1.Remove(selectedMonsters1[random.Next(0, 5)]);
                    selectedMonsters1.Remove(selectedMonsters1[random.Next(0, 4)]);
                    selectedMonsters1.Remove(selectedMonsters1[random.Next(0, 3)]);
                }
            }
            return selectedMonsters1;
        }

        //�������� ���� ���� �� �������
        private void Battle(int EnemyNum)
        {
            player.Attack(selectedMonsters[EnemyNum - 1]); //�÷��̾� ����
            if (selectedMonsters[EnemyNum - 1].Health <= 0)
            {
                deadMonsters.Add(selectedMonsters[EnemyNum - 1]);
                selectedMonsters[EnemyNum - 1].isDead = true; //Deadȸ��ǥ��
                Game.instance.questManager.dicQuestInfos[1].OnCheckEvent(1, 1);
            }
            for (int i = 0; i < selectedMonsters.Count; i++)
            {
                if (!deadMonsters.Contains(selectedMonsters[i]) && !player.SkillFail()) //���ͻ��� + �÷��̾� ��ų���� �Ǵ� �Ϲݰ��� ��ȿ
                    selectedMonsters[i].EnemyAttack(player); //���� ����
            }
        }

        //���� �й� ���
        private void LoseScene()
        {
            Console.WriteLine("===================");
            Console.WriteLine("\nBattle!! - Result");
            Console.WriteLine("\nYou Lose.");
            Console.WriteLine("\n�������� �й��Ͽ����ϴ�.");
            Console.WriteLine($"\nLv.{player.Level} {player.Name}");
            Console.WriteLine($"HP {origin.Health} -> Dead\n"); ////////������ ���� ����
            Console.WriteLine("0. ����\n");
            Game.instance.inputManager.GetValidSelectedIndex(0);
            //���
        }

        private void UseCharacterSkill(Character player, Enemy enemy)
        {
            // ��ų ��� �޼��� ȣ��
            player.UseSkill(enemy);
        }

        private void UseItem()
        {
            Console.WriteLine("===================");
            Console.WriteLine("����� �������� �����ϼ���.");
            List<Item> consumable = new List<Item>();
            foreach (var item in player.InventoryManager.dicInventory)
            {
                if (item.Value.Type == ItemType.Consumable)
                {
                    consumable.Add(item.Value);
                }
            }

            int itemIndex = 1;
            foreach (var item in consumable)
            {
                Console.WriteLine($"- {itemIndex++}. {item.Name} * {item.Count} | {item.Description}");
            }

            int inputkey = Game.instance.inputManager.GetValidSelectedIndex(consumable.Count);
            if (inputkey == 0)
                return;
            player.InventoryManager.AddItemStatBonus(consumable[inputkey - 1]);
            player.InventoryManager.RemoveItem(consumable[inputkey - 1]);
            Thread.Sleep(2000);
        }

        //���� �¸� ȭ��
        private void ClearDungeon()
        {
            deadMonsters.Clear();

            int damage = player.OriginHealth - player.Health;
            Console.WriteLine("===================");
            Console.WriteLine("\nBattle!! - Result");
            Console.WriteLine("\nVictory");
            Console.WriteLine("\n�������� ���� 3������ ��ҽ��ϴ�.");
            Console.WriteLine($"\nLv.{player.Level} {player.Name}");
            Console.WriteLine($"HP {origin.Health} -> {player.Health}");////////������ ���� ����
            Console.WriteLine($"\n�⺻ ����: {dungeon.baseReward} G");
            Console.WriteLine($"\n���� Ŭ����! ü�� {damage} �Ҹ��.");
            Console.WriteLine($"���� ü��: {player.Health}\n");

            player.Exp += 5;       // ���� ����ĥ ������ ����ġ 1 ����
            Console.WriteLine($"\n����ġȹ��: {player.Exp}");

            player.LevelUp.CheckLevelUp();

            Random random = new Random(Guid.NewGuid().GetHashCode());
            if (random.Next(1, 101) <= 20) //15~20% Ȯ���� ������ ���
            {
                DropHighTierItem();
                DropSpecialItem(dungeon.difficulty);
            }
            Console.WriteLine("0. ����\n");
            Console.Write(">>");
            Game.instance.inputManager.GetValidSelectedIndex(0);
        }

        private int DropHighTierItem()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            double percentage = random.Next(10, 21) / 100.0; // 10% ~ 20% ���� ��

            int additionalReward = (int)(player.AttackPower * percentage);  // double ���� int�� ĳ����

            return additionalReward;
        }

        private void DropSpecialItem(Difficulty difficulty)
        {
            // �븻 �������� �ϵ� �������� Ư�� ������ ���
            if (difficulty == Difficulty.Normal ||
                difficulty == Difficulty.Hard)
            {
                int rand = random2.Next(Game.instance.itemManager.specialItems.Count);
                // �������� �ϳ��� ������ ����
                Item droppedItem = Game.instance.itemManager.specialItems[rand];

                Console.WriteLine("===================");
                Console.WriteLine($"Ư���� �������� ȹ���Ͽ����ϴ�: {droppedItem.Name}");
                Console.ReadKey();

                // �ͼ� �������̹Ƿ� Purchased ���� true�� ����
                Game.instance.itemManager.UpdateItemPurchasedStatus(droppedItem);

                // �������� �κ��丮�� ��� ī�װ����� �߰�
                player.InventoryManager.AddItem(droppedItem);
            }
        }
    }
}