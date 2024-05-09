using System;
using System.ComponentModel.Design;

namespace ConsoleGame
{
    public class LevelUp
    {
        private Character character; // Character 클래스 컴포지션

        public LevelUp(Character character)
        {
            this.character = character;
        }

        public void CheckLevelUp()
        {
            character = Game.instance.player
            // 전투에서 이긴 몬스터의 레벨로 가정
            int defeatedMonsterLevel = 1;
            // 몬스터 레벨당 경험치 1로 가정
            int earnedExp = defeatedMonsterLevel;
            // 필요한 경험치 계산  
            int requiredExp = CalculateRequiredExp(character.Level);

            // 경험치가 최대치에 도달하거나 필요한 경험치보다 크거나 같으면 레벨업 가능
            if (character.Exp >= requiredExp || character.Exp >= character.MaxExp)
            {
                // 기본 공격력과 방어력 증가
                character.AttackPower += 4;    // 정수로 변경
                character.DefensePower += 4;  // 정수로 변경

                character.Level++;

                Console.WriteLine($"축하합니다! 레벨 업! 현재 레벨은 Lv{character.Level} 입니다.");
                Console.WriteLine($"기본 공격력이 2, 방어력이 2 증가했습니다.");

                // 경험치를 0으로 초기화
                character.Exp = 0;
                // 최대 경험치 갱신
                character.MaxExp = CalculateRequiredExp(character.Level);
            }
            else if (character.Exp < requiredExp)
            {
                int leftEXP = requiredExp - character.Exp; 
                Console.WriteLine($"레벨을 위한 경험치가 {leftEXP} 부족합니다.");
            }

        }
        private int CalculateRequiredExp(int level)
        {
            // 레벨에 따른 필요 경험치 계산
            return (level - 1) * 5 + 10; 
        }
    }
}