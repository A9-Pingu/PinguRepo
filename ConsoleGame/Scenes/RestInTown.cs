using ConsoleGame.Managers;
using System;

namespace ConsoleGame.Scenes
{
    public class RestInTown
    {
        Character player;
        public RestInTown(Character character)
        {
            player = character;
        }
        public void RestMenu()
        {
            while (true) 
            {
                Game.instance.uiManager.ShowRestMenu();

                //1+1임시 +1해줘야되기때문
                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(1);

                switch (inputKey)
                {
                    case 1:
                        Rest();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        break;
                }
            }
        }

        public void Rest()
        {
            const int restCost = 500;

            if (player.Gold >= restCost)
            {
                player.Gold -= restCost;
                player.Health = player.MaxHealth;
                Console.WriteLine($"휴식을 완료했습니다. 체력이 {player.Health}까지 회복되었습니다.");
            }
            else
            {
                Console.WriteLine("Gold가 부족합니다.");
            }
        }
    }
}
