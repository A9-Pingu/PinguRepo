using System;
using System.Collections.Generic;
using System.Numerics;
using ConsoleGame.Managers;

namespace ConsoleGame
{
    public enum JobType
    {
        전사,
        마법사,
        도적
    }

    public class Character
    {
        public string Name { get; set; }
        public JobType Job { get; set; }
        public int Level { get; set; }
        public LevelUp LevelUp { get; set; }
        public int Exp { get; set; }
        public int MaxExp { get; set; }
        public int AttackPower { get; set; }
        public int DefensePower { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; } = 100;
        public int Gold { get; set; }
        public int OriginHealth { get; set; }  // 기본 체력

        public int AdditionalDamage { get; set; } = 0;

        public int DungeonClearCount { get; private set; } = 0;
        public int MP { get; set; } = 50;

        public InventoryManager InventoryManager { get; set; }

        public delegate void SkillAction(Character player, Enemy enemy, int skillIndex);
        private SkillAction[] SkillSet;

        private readonly string[] WarriorSkills = { "펭귄 슬래시 - MP 10", "크로스 어택 - MP 15" };
        private readonly string[] MageSkills = { "펭귄 행진곡 - MP 15", "아르페지오 - MP 20" };
        private readonly string[] RogueSkills = { "더블 펭펭이 - MP 10", "스프릿 대거 - MP 15" };


        public Character(string name, JobType job)
        {
            Name = name;
            Job = job;
            Level = 1;
            Exp = 0;
            AttackPower = 10;
            DefensePower = 5;
            Gold = 35000;
            Health = MaxHealth;

            // 최대 경험치를 초기화합니다. 예를 들어 레벨이 1일 때 최대 경험치를 설정할 수 있습니다.
            MaxExp = CalculateMaxExp(Level);
            InventoryManager = new InventoryManager(this);
            InitializeSkillSet();
        }

        private void InitializeSkillSet()
        {
            // 각 직업에 따라 스킬을 선택하고 사용합니다.
            SkillSet = new SkillAction[3];
            SkillSet[(int)JobType.전사] = UseWarriorSkill;
            SkillSet[(int)JobType.마법사] = UseMageSkill;
            SkillSet[(int)JobType.도적] = UseRogueSkill;
         }

        private int CalculateMaxExp(int level)
        {
            LevelUp = new LevelUp(this);
            return level * 100;
        }

        public bool HasRequiredDefense(int requiredDefense)
        {
            return DefensePower >= requiredDefense;
        }

        //스킬사용 실패(MP부족)
        bool isSkillFail = false;
        public bool SkillFail()
        {
            if (isSkillFail == true)
                return true;
            else
                return false;
        }

        //플레이어공격(일반,스킬)
        public void Attack(Enemy enemy)
        {
            if (!enemy.IsUseItem())
            {
                Console.WriteLine("===================");
                Console.WriteLine("1. 일반공격");
                Console.WriteLine("2. 스킬공격");
                Console.WriteLine("===================");
                int action = Game.instance.inputManager.GetValidSelectedIndex(2, 1);
                //일반공격
                if (action == 1)
                {
                    Random random = new Random();
                    double percentage = random.NextDouble() * 0.10 - 0.05; //공격력 10% 오차범위
                    int extendAttackPower = (int)(AttackPower * (1 + percentage));
                    isSkillFail = false;

                    // 15% 확률로 치명타 발생
                    bool isCritical = random.Next(1, 101) <= 15;

                    Console.WriteLine("===================");
                    Console.WriteLine($"{Name} 의 공격!");
                    int enemyPreHP = enemy.Health;

                    // 치명타가 발생한 경우
                    if (isCritical)
                    {
                        // 160% 데미지
                        int criticalDamage = (int)(AttackPower * 1.6);
                        Console.WriteLine($"Lv.{enemy.Level} {enemy.Name}에게 {criticalDamage}의 피해를 입혔습니다. - 치명타 공격!!");
                        enemy.Health -= criticalDamage;
                    }
                    else
                    {
                        Console.WriteLine($"Lv.{enemy.Level} {enemy.Name} 에게 {extendAttackPower}의 피해를 입혔습니다. ");
                        enemy.Health -= extendAttackPower;
                    }
                    Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}");
                    if (enemy.Health <= 0)
                    {
                        Console.WriteLine($"HP {enemyPreHP} -> Dead");
                    }
                    else
                        Console.WriteLine($"HP {enemyPreHP} -> {enemy.Health}");
                    Console.WriteLine($"\n0. 다음");
                    Console.Write(">>");
                    Game.instance.inputManager.GetValidSelectedIndex(0);
                }
                //스킬공격
                else
                    UseSkill(enemy);
            }
        }

        //스킬공격
        public void UseSkill(Enemy enemy)
        {
            Console.WriteLine("===================");
            Console.WriteLine($"Lv.{enemy.Level} {enemy.Name} HP {enemy.Health} ATK {enemy.Attack}");
            Console.WriteLine("\n[내정보]");
            Console.WriteLine($"Lv.{Level} {Name} ({Job})");
            Console.WriteLine($"HP {Health}/{MaxHealth}");
            Console.WriteLine($"MP {MP}/50");

            Console.WriteLine("\n[스킬]");
            string[] skills = GetSkillList();
            for (int i = 0; i < skills.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {skills[i]}");
            }
            Console.WriteLine("0. 취소");

            Console.WriteLine("\n사용할 스킬을 선택해주세요.");
            Console.Write(">>");
            int skillchoice;
            while (!int.TryParse(Console.ReadLine(), out skillchoice) || (skillchoice < 0 || skillchoice > skills.Length))
            {
                Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                Console.WriteLine("\n사용할 스킬을 선택해주세요.");
                Console.Write(">>");
            }

            if (skillchoice == 0)
            {
                Console.WriteLine("취소되었습니다.");
                isSkillFail = true;
            }
            else
            {
                // 사용자가 선택한 인덱스에서 1을 빼서 실제 스킬의 인덱스를 계산합니다.
                SkillSet[(int)Job](this, enemy, skillchoice - 1);
            }
        }


        private string[] GetSkillList()
        {
            return Job switch
            {
                JobType.전사 => WarriorSkills,
                JobType.마법사 => MageSkills,
                JobType.도적 => RogueSkills,
                _ => new string[0],
            };
        }

        // 전사 스킬 사용 부분
        private void UseWarriorSkill(Character player, Enemy enemy, int skillIndex)
        {
            // int skillIndex = ChooseSkillIndex(player); // 메서드 내부에서 선언된 skillIndex 변수 제거
            if (skillIndex == -1) return;

            int requiredMP = skillIndex == 0 ? 10 : 15;
            // 펭귄 슬래시와 크로스 어택의 데미지를 각각 2배와 3배로 설정
            int damage = skillIndex == 0 ? player.AttackPower * 2 : player.AttackPower * 3;
            if (player.MP < requiredMP)
            {
                Console.WriteLine("===================");
                Console.WriteLine("MP가 부족하여 스킬을 사용할 수 없습니다.");
                isSkillFail = true;
                Console.WriteLine("\n0. 다음");
                Console.Write(">>");
                Game.instance.inputManager.GetValidSelectedIndex(0);
            }

            // 스킬을 선택하고 필요한 MP 및 데미지 계산 후 스킬을 사용합니다.
            else
            {
                isSkillFail = false;
                string skillName = WarriorSkills[skillIndex];
                Console.WriteLine("===================");
                Console.WriteLine($"MP {requiredMP}를 소모했습니다.");
                Console.WriteLine($"\n{Name}의 {skillName} 공격!!");
                Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}에게 {damage}의 피해를 입혔습니다.");
                int enemyPreHP = enemy.Health;
                enemy.Health -= damage;
                Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}");
                if (enemy.Health <= 0)
                {
                    Console.WriteLine($"HP {enemyPreHP} -> Dead");
                }
                else
                    Console.WriteLine($"HP {enemyPreHP} -> {enemy.Health}");
                Console.WriteLine("\n0. 다음");
                Console.Write(">>");
                Game.instance.inputManager.GetValidSelectedIndex(0);
                player.MP -= requiredMP;
            }
        }

        // 마법사 스킬 사용 부분
        private void UseMageSkill(Character player, Enemy enemy, int skillIndex)
        {
            // int skillIndex = ChooseSkillIndex(player); // 메서드 내부에서 선언된 skillIndex 변수 제거
            if (skillIndex == -1) return;

            int requiredMP = skillIndex == 0 ? 15 : 20;
            // 파이어볼과 아르페지오의 데미지를 각각 3배와 5배로 설정
            int damage = skillIndex == 0 ? player.AttackPower * 3 : player.AttackPower * 5;
            if (player.MP < requiredMP)
            {
                Console.WriteLine("===================");
                Console.WriteLine("MP가 부족하여 스킬을 사용할 수 없습니다.");
                isSkillFail = true;
                Console.WriteLine("\n0. 다음");
                Console.Write(">>");
                Game.instance.inputManager.GetValidSelectedIndex(0);
            }
            // 스킬을 선택하고 필요한 MP 및 데미지 계산 후 스킬을 사용합니다.
            else
            {
                isSkillFail = false;
                string skillName = MageSkills[skillIndex];
                Console.WriteLine("===================");
                Console.WriteLine($"MP {requiredMP}를 소모했습니다.");
                Console.WriteLine($"\n{Name}의 {skillName} 공격!!");
                Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}에게 {damage}의 피해를 입혔습니다.");
                int enemyPreHP = enemy.Health;
                enemy.Health -= damage;
                Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}");
                if (enemy.Health <= 0)
                {
                    Console.WriteLine($"HP {enemyPreHP} -> Dead");
                }
                else
                    Console.WriteLine($"HP {enemyPreHP} -> {enemy.Health}");
                Console.WriteLine("\n0. 다음");
                Console.Write(">>");
                Game.instance.inputManager.GetValidSelectedIndex(0);
                player.MP -= requiredMP;
            }
        }

        // 도적 스킬 사용 부분
        private void UseRogueSkill(Character player, Enemy enemy, int skillIndex)
        {
            // int skillIndex = ChooseSkillIndex(player); // 메서드 내부에서 선언된 skillIndex 변수 제거
            if (skillIndex == -1) return;

            int requiredMP = skillIndex == 0 ? 10 : 15;
            // 더블 펭펭이와 스프릿 대거의 데미지를 각각 2배와 4배로 설정
            int damage = skillIndex == 0 ? player.AttackPower * 2 : player.AttackPower * 4;
            if (player.MP < requiredMP)
            {
                Console.WriteLine("===================");
                Console.WriteLine("MP가 부족하여 스킬을 사용할 수 없습니다.");
                isSkillFail = true;
                Console.WriteLine("\n0. 다음");
                Console.Write(">>");
                Game.instance.inputManager.GetValidSelectedIndex(0);
            }
            // 스킬을 선택하고 필요한 MP 및 데미지 계산 후 스킬을 사용합니다.
            else
            {
                isSkillFail = false;
                string skillName = RogueSkills[skillIndex];
                Console.WriteLine("===================");
                Console.WriteLine($"MP {requiredMP}를 소모했습니다.");
                Console.WriteLine($"\n{Name}의 {skillName} 공격!!");
                Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}에게 {damage}의 피해를 입혔습니다.");
                int enemyPreHP = enemy.Health;
                enemy.Health -= damage;
                Console.WriteLine($"\nLv.{enemy.Level} {enemy.Name}");
                if (enemy.Health <= 0)
                {
                    Console.WriteLine($"HP {enemyPreHP} -> Dead");
                }
                else
                    Console.WriteLine($"HP {enemyPreHP} -> {enemy.Health}");
            }
            Console.WriteLine("\n0. 다음");
            Console.Write(">>");
            Game.instance.inputManager.GetValidSelectedIndex(0);
            player.MP -= requiredMP;
        }

        private int ChooseSkillIndex(Enemy enemy)
        {
            Console.Write("\n원하시는 행동을 입력해주세요: ");
            int skillChoice;
            while (!int.TryParse(Console.ReadLine(), out skillChoice) || (skillChoice < 0 || skillChoice > 2))
            {
                Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                Console.Write("원하시는 행동을 입력해주세요: ");               
            }

            Console.WriteLine($"당신이 {enemy.Name}에게 {AttackPower}의 피해를 입혔습니다.");
            enemy.Health -= AttackPower;
            return skillChoice;
        }

        public void UseItem(Item item, int count = 1)
        {
            if (item.Type != ItemType.Consumable) return;
            InventoryManager.AddItemStatBonus(item);
            InventoryManager.RemoveItem(item, count);
            Console.WriteLine($"{item.Name}을 {count}개를 사용하였습니다.");
        }

    }
}
