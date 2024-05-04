using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame.Managers
{
    enum EScene
    {
        e_Exit,
        e_Status,
        e_Inventory,
        e_Shop,
        e_Dungeon,
        e_Inn,
        e_Guild,
        e_Save,
        e_Load,
        e_MaxNum
    }

    public enum Difficulty
    {
        Easy = 1,
        Normal,
        Hard,
        Max
    }

    internal class Define
    {
    }
}
