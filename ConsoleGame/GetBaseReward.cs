using System;
using ConsoleGame.Managers;

namespace ConsoleGame
{
    public class GetBaseReward
    {
        public int Calculate(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    return 1000;
                case Difficulty.Normal:
                    return 2000;
                case Difficulty.Hard:
                    return 3500;
                default:
                    throw new ArgumentException("Invalid difficulty");
            }
        }
    }
}