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
        ASCIIArt aSCIIArt = new ASCIIArt();
        private Character player;
        private Random random = new Random();
        private Random random2 = new Random();
        public Character origin; ////////던전에 깊은 복사
        private Dungeon dungeon;
        bool useItem = false;
        int monsterIndex = 0;
        // 몬스터 도감
        List<Enemy> allMonsters = new List<Enemy>
        {
            new Enemy("도둑갈매기", 2),
            new Enemy("야생들개", 2),
            new Enemy("여우", 2),
            new Enemy("바다표범", 3),
            new Enemy("늑대", 3),
            new Enemy("북극곰", 4),
            new Enemy("범고래", 5),
        };
        List<Enemy> deadMonsters = new List<Enemy>(); //던전에 깊은 복사
        List<Enemy> selectedMonsters = new List<Enemy>();
        Enemy[] enemies = new Enemy[3]
        {
            new Enemy(),
            new Enemy(),
            new Enemy()
        };

        public DungeonScene(Character character)
        {
            player = character;
            origin = player.DeepCopy(); //던전에 깊은 복사
            origin.Health = player.Health; //던전에 깊은 복사
        }

        //던전 입장 조건
        public void EnterDungeon()
        {
            while (true)
            {
                Console.Clear();
                aSCIIArt.BattleImage();
                Console.WriteLine("===================");
                Console.WriteLine("1. 쉬운 던전     | 누구나 가능");
                Console.WriteLine("2. 일반 던전     | 방어력 30 이상 권장");
                Console.WriteLine("3. 어려운 던전    | 방어력 50 이상 권장");
                Console.WriteLine("0. 나가기");
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");

                int InputKey = Game.instance.inputManager.GetValidSelectedIndex((int)Difficulty.Max - 1);
                dungeon = new Dungeon((Difficulty)InputKey);

                if (InputKey == 0)
                    return;

                else if (InputKey != 0 && dungeon.requiredDefense < player.DefensePower)
                {
                    Start(dungeon.difficulty);
                }
                else
                {
                    Console.WriteLine($"방어력이 {dungeon.requiredDefense} 이상이어야 {dungeon.difficulty} 던전에 입장할 수 있습니다.");
                    Game.instance.inputManager.InputAnyKey();
                    Console.Clear();
                }
            }
        }



        public void Start(Difficulty difficulty)
        {
            origin.Health = player.Health; //플레이어 초기 체력
            SelectMonsters(difficulty);
            Game.instance.uiManager.BattleScene(difficulty, selectedMonsters, player, false); //초기화면
            Game.instance.inputManager.GetValidSelectedIndex(1, 1);
            while (true)
            {
                Game.instance.uiManager.BattleScene(difficulty, selectedMonsters, player, true);
                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(selectedMonsters.Count + 2);
                if (inputKey == 0)
                {
                    Console.WriteLine("===================");
                    Console.WriteLine("이대로 던전을 나가시겠습니까?");
                    Console.WriteLine("1. 네");
                    Console.WriteLine("0. 아니오");
                    int nextKey = Game.instance.inputManager.GetValidSelectedIndex(1);
                    if (nextKey == 1)
                        return;
                    else
                        continue;
                }
                Battle(inputKey); //배틀 시작
                if (player.Health <= 0) //플레이어 사망
                {
                    LoseScene();
                    return;
                }
                else if (deadMonsters.Count == monsterIndex) //모든 몬스터 사망 시
                {
                    ClearDungeon();
                    return;
                }
            }
        }



        private void SelectMonsters(Difficulty difficulty)
        {
            selectedMonsters.Clear();
            //난이도별 랜덤 몬스터 3마리 선택
            (int,int) difficultyIndex = difficulty switch
            {
                Difficulty.Easy => (0, 3),
                Difficulty.Normal => (2, 5),
                Difficulty.Hard => (4, 7),
                _ => (0, 0)
            };

            monsterIndex = random.Next(1, 3);
            int rand = 0;
            for (int i = 0; i < monsterIndex; i++)
            {
                selectedMonsters.Add(enemies[i]);
                rand = random.Next(difficultyIndex.Item1, difficultyIndex.Item2);
                selectedMonsters[i].InitEnemy(allMonsters[rand]);
            }
        }

        //던전에서 공격 시작 후 전투장면
        private void Battle(int EnemyNum)
        {
            if (EnemyNum == selectedMonsters.Count + 1)
            {
                UseItem();
            }
            else
            {
                player.Attack(selectedMonsters[EnemyNum - 1]); //플레이어 공격
                if (selectedMonsters[EnemyNum - 1].Health <= 0)
                {
                    deadMonsters.Add(selectedMonsters[EnemyNum - 1]);
                    selectedMonsters[EnemyNum - 1].isDead = true; //Dead회색표시
                    Game.instance.questManager.dicQuestInfos[1].OnCheckEvent(1, 1);
                }
            }

            for (int i = 0; i < selectedMonsters.Count; i++)
            {
                if (!deadMonsters.Contains(selectedMonsters[i]) && !player.SkillFail()) //몬스터생존 + 플레이어 스킬공격 또는 일반공격 유효
                    selectedMonsters[i].EnemyAttack(player); //몬스터 공격
            }
        }

        //전투 패배 장면
        private void LoseScene()
        {
            Console.WriteLine("===================");
            Console.WriteLine("\nBattle!! - Result");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nYou Lose.");
            Console.ResetColor();
            Console.WriteLine("\n전투에서 패배하였습니다.");
            Console.WriteLine($"\nLv.{player.Level} {player.Name}");
            Console.WriteLine($"HP {origin.Health} -> Dead\n"); ////////던전에 깊은 복사
            Console.WriteLine("0. 다음\n");
            Game.instance.inputManager.GetValidSelectedIndex(0);
            //대기
        }

        private void UseCharacterSkill(Character player, Enemy enemy)
        {
            // 스킬 사용 메서드 호출
            player.UseSkill(enemy);
        }

        private void UseItem()
        {
            Console.WriteLine("===================");
            Console.WriteLine("사용할 아이템을 선택하세요.");
            List<Item> consumable = new List<Item>();
            foreach (var item in player.InventoryManager.dicInventory)
            {
                if (item.Value.Type == ItemType.Consumable)
                {
                    consumable.Add(item.Value);
                }
            }

            int itemIndex = 1;
            if (consumable.Count > 0)
            {
                foreach (var item in consumable)
                {
                    Console.WriteLine($"- {itemIndex++}. {item.Name} * {item.Count} | {item.Description}");
                }
            }
            else
            {
                Console.WriteLine("사용할 수 있는 아이템이 없습니다..");
                Thread.Sleep(500);
                return;
            }

            int inputkey = Game.instance.inputManager.GetValidSelectedIndex(consumable.Count);
            if (inputkey == 0)
                return;
            player.UseItem(consumable[inputkey - 1]);
            Thread.Sleep(2000);
        }

        //전투 승리 화면
        private void ClearDungeon()
        {
            deadMonsters.Clear();

            int damage = origin.Health - player.Health;
            Console.WriteLine("===================");
            Console.WriteLine("\nBattle!! - Result");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nVictory");
            Console.ResetColor();
            Console.WriteLine("\n던전에서 몬스터 3마리를 잡았습니다.");
            Console.WriteLine($"\nLv.{player.Level} {player.Name}");
            Console.WriteLine($"HP {origin.Health} -> {player.Health}");////////던전에 깊은 복사
            Console.WriteLine($"\n기본 보상: {dungeon.baseReward} G");
            Console.WriteLine($"\n던전 클리어! 체력 {damage} 소모됨.");
            Console.WriteLine($"남은 체력: {player.Health}\n");

            player.Exp += 5;       // 적을 물리칠 때마다 경험치 1 증가
            Console.WriteLine($"\n경험치획득: {player.Exp}");

            player.LevelUp.CheckLevelUp();

            Random random = new Random(Guid.NewGuid().GetHashCode());
            if (random.Next(1, 101) <= 20) //15~20% 확률로 아이템 드롭
            {
                DropHighTierItem();
                DropSpecialItem(dungeon.difficulty);
            }
            Console.WriteLine("0. 다음\n");
            Console.Write(">>");
            Game.instance.inputManager.GetValidSelectedIndex(0);
        }

        private int DropHighTierItem()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            double percentage = random.Next(10, 21) / 100.0; // 10% ~ 20% 랜덤 값

            int additionalReward = (int)(player.AttackPower * percentage);  // double 값을 int로 캐스팅

            return additionalReward;
        }

        private void DropSpecialItem(Difficulty difficulty)
        {
            // 노말 던전부터 하드 던전까지 특별 아이템 드롭
            if (difficulty == Difficulty.Normal ||
                difficulty == Difficulty.Hard)
            {
                int rand = random2.Next(Game.instance.itemManager.specialItems.Count);
                // 무작위로 하나의 아이템 선택
                Item droppedItem = Game.instance.itemManager.specialItems[rand];

                Console.WriteLine("===================");
                Console.WriteLine($"특별한 아이템을 획득하였습니다: {droppedItem.Name}");
                Console.ReadKey();

                // 귀속 아이템이므로 Purchased 값을 true로 설정
                Game.instance.itemManager.UpdateItemPurchasedStatus(droppedItem);

                // 아이템을 인벤토리의 장비 카테고리에 추가
                player.InventoryManager.AddItem(droppedItem);
            }
        }
    }
}