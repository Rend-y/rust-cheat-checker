using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace RCC
{
    public class detecting_cleaning
    {
        readonly List<string> all_logs_detected_cleaning = new List<string>();
        readonly List<Tuple<string, string, string>> folder_and_file_for_clear = new List<Tuple<string, string, string>>()
        {
            Tuple.Create($"C:\\Users\\{Environment.UserName}\\Recent","", "Recent"),
            Tuple.Create("C:\\Windows\\Prefetch\\Prefetch","", "Prefetch"),
        };
        private void detect_clear_protected_folder()
        {
            foreach(Tuple<string, string, string> folder in folder_and_file_for_clear)
            {
                string[] files = Directory.GetFiles(folder.Item1, folder.Item2);
                if (files.Count() <= 5)
                {
                    all_logs_detected_cleaning.Add($"Обнаружена очищенная папка {folder.Item3}");
                    return;
                }

                DateTime date_time = DateTime.Now;
                files.ToList().ForEach((get_file) =>
                {
                    DateTime creationTime = File.GetCreationTime(get_file);
                    if ((date_time - creationTime).TotalMinutes > 0.0)
                        date_time = creationTime;
                });
                int totalMinutes = (int)(DateTime.Now - date_time).TotalMinutes;
                if (totalMinutes < 45.0)
                    all_logs_detected_cleaning.Add($"Обнаружена попытка очистки папки {folder.Item3}");
                else if (totalMinutes < 360) all_logs_detected_cleaning.Add($"Самый первый файл в {folder.Item3} был создан {totalMinutes} минут назад");

            }
        }
        private void detect_clear_steam_account()
        {
            string fileDataFromConfig = File.ReadAllText(Steam.local_info.GetPathToConfig());
            List<string> getSteamIdDataFromConfig = Steam.local_info.GetAllSteamId(fileDataFromConfig);
            string fileDataFromLoginUser = File.ReadAllText(Steam.local_info.GetPathToLoginUser());
            List<string> getSteamIdDataFromLoginUser = Steam.local_info.GetAllSteamId(fileDataFromLoginUser);
            if (getSteamIdDataFromConfig.Count == getSteamIdDataFromLoginUser.Count)
                return;

            int accountDetect = getSteamIdDataFromConfig.Count - getSteamIdDataFromLoginUser.Count;
            all_logs_detected_cleaning.Add($"Обнаружено {Math.Abs(accountDetect)} удалённых аккаунтов");
        }
        public void search_all()
        {
            this.detect_clear_steam_account();
            this.detect_clear_protected_folder();
            if (all_logs_detected_cleaning.Count == 0)
                return;
            string message = string.Empty;
            all_logs_detected_cleaning.ForEach(messages => message += $"{messages}\n");
            MessageBox.Show(message);
        }
    }
}
