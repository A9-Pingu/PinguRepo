using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame.Managers
{
    public class InputManager
    {
        public int GetValidSelectedIndex(int maxIndex, int minIndex = 0)
        {
            int selectedIndex;
            while (true)
            {
                bool getkey = int.TryParse(Console.ReadLine(), out selectedIndex);
                if (selectedIndex >= minIndex && selectedIndex <= maxIndex && getkey)
                {
                    return selectedIndex;
                }
                else
                {
                    Console.WriteLine("유효한 번호를 입력해주세요.");
                }
            }
        }

        public void InputAnyKey()
        {
            Console.WriteLine("아무 키나 누르면 계속...");
            Console.ReadKey();
        }
    }
}
