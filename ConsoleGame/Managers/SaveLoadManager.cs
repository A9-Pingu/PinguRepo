using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;

namespace ConsoleGame.Managers
{
    public sealed class SaveLoadManager
    {
        ASCIIArt aSCIIArt = new ASCIIArt();
        private const string SAVE_FOLDER = "SaveGames";
        public SaveData saveData { get; set; } = new SaveData();

        public SaveLoadManager() {}

        public void InitData()
        {
            Game.instance.player = saveData.player;
            Game.instance.questManager = saveData.questManager;
            Game.instance.itemManager = saveData.itemManager;
        }
        public SaveData LoadOrStartGame(InputManager input)
        {
            List<string> savedGames = GetSavedGames();
            aSCIIArt.MainImage();

            if (savedGames.Count > 0)
            {
                DisplaySavedGames(savedGames);
                int selectedIndex = input.GetValidSelectedIndex(savedGames.Count, 1);

                string selectedGame = savedGames[selectedIndex - 1];
                saveData = LoadGame(selectedGame);
                InitData();
                return saveData;
            }
            else
            {
                Console.WriteLine("저장된 게임 파일이 없습니다.");
                saveData.player = CreateNewCharacter();
                return saveData;
            }            
        }


        public List<string> GetSavedGames()
        {
            string saveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), SAVE_FOLDER);

            if (!Directory.Exists(saveFolderPath))
            {
                return new List<string>();
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");

            List<string> savedGames = new List<string>();
            foreach (var file in files)
            {
                savedGames.Add(file.Name);
            }

            return savedGames;
        }

        public void SaveGame(SaveData saveData)
        {
            string saveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), SAVE_FOLDER);

            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }

            // 디렉터리 내의 파일을 가져옵니다.
            string[] files = Directory.GetFiles(saveFolderPath);

            // 파일 개수 확인
            if (files.Length >= 6)
            {
                // 파일을 작성된 시간순으로 정렬
                Array.Sort(files, (a, b) => File.GetLastWriteTime(a).CompareTo(File.GetLastWriteTime(b)));

                // 오래된 파일부터 삭제
                for (int i = 0; i < files.Length - 6; i++)
                {
                    File.Delete(files[i]);
                    Console.WriteLine($"게임 파일 {Path.GetFileName(files[i])}을(를) 삭제하였습니다.");
                }
            }

            string saveName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            string filePath = Path.Combine(saveFolderPath, $"{saveName}.json");

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, settings);
            File.WriteAllText(filePath, json);

            Console.WriteLine($"게임이 저장되었습니다. ({saveName}.json)");
        }


        public SaveData LoadGame(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), SAVE_FOLDER, fileName);
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    saveData = JsonConvert.DeserializeObject<SaveData>(json);
                    Console.WriteLine("게임 불러오기 완료");
                    return saveData;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"게임 불러오기 실패: {e.Message}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("선택한 게임 파일이 없습니다.");
                return null;
            }
        }

        public bool IsValidJob(string jobString)
        {
            // 입력된 직업이 유효한지 확인합니다.
            string[] validJobs = { "전사", "마법사", "도적" };
            return validJobs.Contains(jobString);
        }

        Character CreateNewCharacter()
        {
            Console.Write("캐릭터 이름을 입력하세요: ");
            string name = Console.ReadLine();

            Console.Write("직업을 선택하세요 (전사, 마법사, 도적 등): ");
            string jobString = Console.ReadLine();

            // 유효한 직업이 입력될 때까지 사용자에게 다시 입력 요청
            while (!IsValidJob(jobString))
            {
                Console.WriteLine("유효하지 않은 직업입니다. 다시 입력해주세요.");
                Console.Write("직업을 선택하세요 (전사, 마법사, 도적 등): ");
                jobString = Console.ReadLine();
            }

            // 입력된 직업 문자열을 JobType enum으로 변환
            JobType job = Enum.Parse<JobType>(jobString, true);
            return new Character(name, job);
        }

        public void DeleteSavedGame(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), SAVE_FOLDER, fileName);
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Console.WriteLine($"게임 파일 {fileName}을(를) 삭제하였습니다.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"게임 파일 삭제 실패: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine("선택한 게임 파일이 없습니다.");
            }
        }

        public void DisplaySavedGames(List<string> savedGames)
        {
            Console.WriteLine("불러올 게임을 선택해주세요:");
            for (int i = 0; i < savedGames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {savedGames[i]}");
            }
        }


    }

    public class SaveData
    {
        public ItemManager itemManager;
        public QuestManager questManager;
        public Character player;
    }
}
