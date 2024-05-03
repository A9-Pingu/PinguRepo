using System;
using System.Collections.Generic;
using ConsoleGame.Managers;
using ConsoleGame.Scenes;

namespace ConsoleGame
{
    public class Game
    {
        public static Game instance { get; private set; }
        #region //게임 매니저
        public SaveLoadManager saveLoadManager { get; private set; }
        public UIManager uiManager { get; set; }
        public InputManager inputManager { get; set; }
        public ItemManager itemManager { get; set; }
        #endregion
        #region //게임 씬
        public RestInTown restScene { get; set; }
        public Shop shopScene { get; set; }
        public DungeonScene dungeon { get; set; }
        #endregion
        public Character player;
        
        public bool isPlaying = true;
        public Game()
        {
            if (instance == null)
            {
                instance = this;
            }
            uiManager = new UIManager();
            inputManager = new InputManager();
            saveLoadManager = new SaveLoadManager();
            player = saveLoadManager.LoadOrStartGame(inputManager);
            itemManager = new ItemManager();
            shopScene = new Shop(player, itemManager);
            restScene = new RestInTown(player);
            dungeon = new DungeonScene(player);
            questManager = new QuestManager();
            questManager.InitQuest();
            guild = new GuildScene(player);
        }

        public void Run()
        {          
            while (isPlaying)
            {
                uiManager.DisplayMainMenu();
                int choice = inputManager.GetValidSelectedIndex((int)EScene.e_MaxNum - 1);
                isPlaying = SelectScene(choice);
            }
        }


        bool SelectScene(int choice)
        {
            switch (choice)
            {
                case (int)EScene.e_Exit:
                    Console.WriteLine("게임을 종료합니다.");
                    return false;
                case (int)EScene.e_Status:
                    uiManager.ShowStatus(player);
                    break;
                case (int)EScene.e_Inventory:
                    player.InventoryManager.ShowInventory();
                    break;
                case (int)EScene.e_Shop:
                    shopScene.ShowShop();
                    break;
                case (int)EScene.e_Dungeon:
                    dungeon.EnterDungeon();
                    break;
                case (int)EScene.e_Inn:
                    restScene.RestMenu();
                    break;
                case (int)EScene.e_Guild:
                    break;
                case (int)EScene.e_Save:
                    saveLoadManager.SaveGame(player);
                    Console.WriteLine("게임이 저장되었습니다.");
                    Console.ReadKey();
                    break;
                case (int)EScene.e_Load:
                    player = saveLoadManager.LoadOrStartGame(inputManager);
                    break;
                default:
                    Console.WriteLine("잘못된 선택입니다.");
                    break;
            }
            return true;
        }
    }
}
