using System;
using System.Collections.Generic;
using ConsoleGame.Managers;

namespace ConsoleGame
{
    public class Dungeon
    {
        public Difficulty difficulty;
        public int baseReward;
        public int requiredDefense;

        public Dungeon(Difficulty Step)
        {
            difficulty = Step;
            SetBaseReward();
            SetRequiredDefense();
        }

        private void SetBaseReward()
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    baseReward = 1000;
                    break;
                case Difficulty.Normal:
                    baseReward = 2000;
                    break;
                case Difficulty.Hard:
                    baseReward = 3500;
                    break;
                default:
                    Console.WriteLine("잘못된 입력입니다");
                    break;
            }
        }


        public int SetRequiredDefense()
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return 0;
                case Difficulty.Normal:
                    return 30;
                case Difficulty.Hard:
                    return 50;
                default:
                    return 0;
            }
        }
    }
}