using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGame
{
    public enum EEventType
    {
        MonsterKill,
        Equipment,
        StatusUp
    }

    public class QuestData : EventArgs
    {
        public int Index;
        public int Value1;
        public int Value2;
        public QuestData(int index, int value)
        {
            Index = index;
            Value1 = value;
        }

        public QuestData(int index, int value1, int value2)
        {
            Index = index;
            Value1 = value1;
            Value2 = value2;
        }
    }

    public class Quest
    {
        public int Keycode = 0;
        public string Title = "공 백";
        public string Content = string.Empty;
        public bool IsAccept = false;
        public bool IsClear = false;
        public bool IsEnd = false;
        public string ClearContent = string.Empty;
        public int NowStack = 0;
        public int ClearStack = 0;
        public int Gold = 0;
        public Item Reward;
        public int ItemConunt;

        //튜플(tuple) (int,int)
        public event EventHandler OnCheckEventHandler;
        public Quest(int key, string title, string content, string clearContent, int clearStack, int gold = 0, Item reward = null)
        {
            InitQuest(key, title, content, clearContent, clearStack, gold, reward);
        }

        public void InitQuest(int key, string title, string content, string clearContent, int clearStack, int gold = 0, Item reward = null, int itemCount = 1)
        {
            Keycode = key;
            Title = title;
            Content = content;
            ClearContent = clearContent;
            ClearStack = clearStack;
            Gold = gold;
            Reward = reward;
            ItemConunt = itemCount;
        }

        public void CheckedQuest(object sender, EventArgs e)
        {
            QuestData questData = (QuestData)e;

            if (!IsClear)
            {
                switch (questData.Index)
                {
                    case 1:
                        NowStack++;
                        break;
                    case 2:
                        NowStack++;
                        break;
                    case 3:
                        NowStack = (int)(questData.Value1 + questData.Value2) / 2;
                        break;
                }
                
                if (NowStack >= ClearStack)
                {
                    IsClear = true;
                    this.OnCheckEventHandler -= CheckedQuest;
                    return;
                }
            }            
        }

        //value오버로드하기
        public void OnCheckEvent(int index, int value)
        {
            OnCheckEventHandler?.Invoke(this, new QuestData(index, value));            
        }

        public void OnCheckEvent(int index, int value1, int value2)
        {
            OnCheckEventHandler?.Invoke(this, new QuestData(index, value1, value2));
        }

        public void ReceiveReward(Character player)
        {
            if (Gold > 0)
            {
                player.Gold += Gold;
                Console.WriteLine($"의뢰 해결 보상으로 {Gold}G를 획득하였습니다!");
            }

            if (Reward != null)
            {
                player.InventoryManager.AddItem(Reward, ItemConunt);
                Console.WriteLine($"의뢰 해결 비용으로 {Reward.Name}을/를 획득하였습니다!");
            }
            Console.WriteLine("의뢰를 성공적으로 달성하였습니다! 축하합니다!");
            Console.WriteLine("오늘 한탕 크게 벌었으니 한 턱 내라고~~");
            Game.instance.inputManager.InputAnyKey();
        }
    }
}
