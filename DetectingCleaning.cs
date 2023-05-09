using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using MessageBox = RCC.windows.MessageBox;

namespace RCC
{
    public struct CleaningFolderData
    {
        public string FullPath { get; set; }
        public string SearchPattern { get; set; }
        public string FolderName { get; set; }
        public CleaningFolderData(string fullPath, string any, string folderName)
        {
            this.FullPath = fullPath;
            this.SearchPattern = any;
            this.FolderName = folderName;
        }
    }
    public class DetectingCleaning
    {
        readonly List<string> _allLogsDetectedCleaning = new List<string>();
        readonly List<CleaningFolderData> _folderAndFileForClear = new List<CleaningFolderData>()
        { 
            new CleaningFolderData($"C:\\Users\\{Environment.UserName}\\Recent","", "Recent"),
            new CleaningFolderData("C:\\Windows\\Prefetch\\Prefetch","", "Prefetch"),
        };
        private void DetectClearProtectedFolder()
        {
            foreach(CleaningFolderData folder in _folderAndFileForClear)
            {
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folder.FullPath);
                    FileInfo[] files = directoryInfo.GetFiles();
                    int filesCount = files.Count();
                    if (filesCount <= 15)
                    {
                        _allLogsDetectedCleaning.Add($"Обнаружена очищенная папка {folder.FolderName}");
                        _allLogsDetectedCleaning.Add($"Текушие количество файлов в папке {folder.FolderName}: {filesCount}");
                        return;
                    }
                    DateTime dateTime = DateTime.Now;
                    files.ToList().ForEach(file =>
                    {
                        if ((dateTime - file.CreationTime).TotalMinutes > 0.0)
                            dateTime = file.CreationTime;
                    });
                    int totalMinutes = (int)(DateTime.Now - dateTime).TotalMinutes;
                    if (totalMinutes < 45.0)
                        _allLogsDetectedCleaning.Add($"Обнаружена попытка очистки папки {folder.FolderName}");
                    else if (totalMinutes < 360)
                        _allLogsDetectedCleaning.Add($"Самый первый файл в {folder.FolderName} был создан {totalMinutes} минут назад");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show($"Не удалось получить доступ к папке {folder.FolderName}\nПерезапустите программу от имени администратора");
                    return;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Неизвестная ошибка\n" +
                                    $"При попытке получить доступ к папке {folder.FolderName}\n" + 
                                    "Пожалуйста сообщите об этой ошибке разработчику\n" +
                                    $"Приложите скриншот этого окна\n" +
                                    $"Сообщение об ошибке:\n{e.Message}\n" );
                    return;
                }
            }
        }
        private void DetectClearSteamAccount()
        {
            string fileDataFromConfig = File.ReadAllText(Steam.LocalInfo.GetPathToConfig);
            string fileDataFromLoginUser = File.ReadAllText(Steam.LocalInfo.GetPathToLoginUser);
            List<string> getSteamIdDataFromConfig = Steam.LocalInfo.GetSteamIsFromContent(fileDataFromConfig);
            List<string> getSteamIdDataFromLoginUser = Steam.LocalInfo.GetSteamIsFromContent(fileDataFromLoginUser);
            List<string> getSteamIdFromCoPlay = Steam.LocalInfo.GetSteamIdFromCoPlay();
            int calculate = getSteamIdDataFromConfig.Count - getSteamIdDataFromLoginUser.Count -
                            getSteamIdFromCoPlay.Count;
            if (getSteamIdDataFromConfig.Count == Math.Abs(calculate))
                return;
            _allLogsDetectedCleaning.Add($"Обнаружены удаленные аккаунты");
        }

        private void SearchAll()
        {
            this.DetectClearSteamAccount();
            this.DetectClearProtectedFolder();
            if (_allLogsDetectedCleaning.Count == 0)
                return;
            string message = string.Empty;
            _allLogsDetectedCleaning.ForEach(messages => message += $"{messages}\n");
            MessageBox.Show(message);
        }
        public static void Start()
        {
            Thread thread = new Thread(() =>
            {
                DetectingCleaning detectingCleaning = new DetectingCleaning();
                detectingCleaning.SearchAll();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
