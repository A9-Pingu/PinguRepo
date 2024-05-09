using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleGame.Scenes
{
    public class GuildScene
    {
        Dictionary<int, Quest> dicQuests;
        Character player;

        public GuildScene(Character character)
        {
            player = character;
            dicQuests = Game.instance.questManager.dicQuestInfos;
            for (int i = 0; i < character.DicQuests.Count; i++)
            {
                dicQuests[i] = character.DicQuests[i];
            }
        }
        public void GuildMenu()
        {
            dicQuests = Game.instance.questManager.dicQuestInfos;
            while (true) 
            {
                Game.instance.uiManager.ShowGuildMenu();
                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(2);
                switch (inputKey)
                {
                    case 0:
                        Console.WriteLine("길드에서 나갑니다.");
                        Thread.Sleep(1000);
                        return;
                    case 1:
                        CheckedQuestCondition();
                        break;
                    default:
                        break;
                }
                Thread.Sleep(1000);
            }
        }

        public void CheckedQuestCondition()
        {
            while(true) 
            {
                Console.Clear();
                Console.WriteLine("\n모험가 길드 - 의뢰 목록 확인\n\n");
                int i = 1;
                foreach (var quest in dicQuests) 
                {                    
                    Console.Write($"{i++}. {quest.Value.Title}");
                    if (quest.Value.IsEnd)
                        Console.Write("\t - 보상완료");
                    else if(quest.Value.IsClear)
                        Console.Write("\t - 보상받기");
                    else if(quest.Value.IsAccept)
                        Console.Write("\t - 진행 중");
                    Console.WriteLine("");
                }
                Console.WriteLine("0. 나가기\n");
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");

                int inputKey = Game.instance.inputManager.GetValidSelectedIndex(dicQuests.Count + 1);
                if (inputKey == 0)
                {
                    Console.WriteLine("길드회관으로 돌아갑니다...");
                    return;
                }

                ShowQuesInfo(dicQuests[inputKey]);
                Thread.Sleep(1000);
            }
        }

        public void ShowQuesInfo(Quest quest)
        {
            Console.Clear();
            Console.WriteLine($"Quest - {quest.Title}");

            Console.WriteLine($"\n\n{quest.Content}");
            if (!quest.IsClear)
            {
                Console.WriteLine($"\n- {quest.ClearContent} ({quest.NowStack} / {quest.ClearStack})");
            }
            else
            {
                Console.WriteLine($"\n- {quest.ClearContent} 퀘스트 완료!!\n\t보상을 수령하세요");
            }

            Console.WriteLine($"\n< 보상 >");
            if(quest.Gold != 0)
                Console.WriteLine($"- {quest.Gold} G");
            if (quest.Reward != null)
                Console.WriteLine($"- {quest.Reward.Name} * {quest.ItemConunt}");
            int index = 0;
            if (quest.IsEnd)
            {
                Console.WriteLine("\n1. 종료된 의뢰입니다.");
                index = 0;
            }
            else if (!quest.IsAccept)
            {
                Console.WriteLine("\n1. 의뢰 수락하기");
                Console.WriteLine("2. 의뢰 거절하기");
                index = 2;
            }
            else if (quest.IsClear)
            {
                Console.WriteLine("\n1. 보상 수령하기");
                index = 1;
            }
            else
            {
                Console.WriteLine("\n퀘스트 진행중...\n");
                index = 0;
            }
            Console.WriteLine("0. 나가기\n");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");

            int inputKey = Game.instance.inputManager.GetValidSelectedIndex(index);
            switch(inputKey) 
            { 
                case 0:
                    Console.WriteLine("의뢰 확인을 그만합니다.");
                    return;
                case 1:
                    QuestClearCheck(quest);
                    break;
                case 2:
                    Console.WriteLine("에잉..쯧... 요즘 애들은 근성이 없어요 근성이!");
                    return;
                default :
                    return;
            }
        }

        public void QuestClearCheck(Quest quest)
        {
            if (!quest.IsAccept)
            {
                //의뢰 수락
                quest.IsAccept = true;
                quest.OnCheckEventHandler -= quest.CheckedQuest;
                quest.OnCheckEventHandler += quest.CheckedQuest;
                Console.WriteLine("의뢰가 성공적으로 접수되었습니다.");
            }
            else if (quest.IsClear && !quest.IsEnd)
            {
                quest.IsEnd = true;
                //보상 받기
                quest.ReceiveReward(player);
            }
        }
    }


}
