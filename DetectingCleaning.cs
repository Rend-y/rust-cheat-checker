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

                DateTime dateTime = DateTime.Now;
                files.ToList().ForEach((get_file) =>
                {
                    DateTime creationTime = File.GetCreationTime(get_file);
                    if ((dateTime - creationTime).TotalMinutes > 0.0)
                        dateTime = creationTime;
                });
                int total_minutes = (int)(DateTime.Now - dateTime).TotalMinutes;
                if (total_minutes < 45.0)
                    all_logs_detected_cleaning.Add($"Обнаружена попытка очистки папки {folder.Item3}");
                else if (total_minutes < 360) all_logs_detected_cleaning.Add($"Самый первый файл в {folder.Item3} был создан {total_minutes} минут назад");

            }
        }
        private void detect_clear_steam_account()
        {
            string file_data_from_config = File.ReadAllText(Steam.local_info.get_path_to_config());
            List<string> get_steam_id_data_from_config = Steam.local_info.get_all_steam_id(file_data_from_config);
            string file_data_from_login_user = File.ReadAllText(Steam.local_info.get_path_to_login_user());
            List<string> get_steam_id_data_from_login_user = Steam.local_info.get_all_steam_id(file_data_from_login_user);
            if (get_steam_id_data_from_config.Count == get_steam_id_data_from_login_user.Count)
                return;

            int account_detect = get_steam_id_data_from_config.Count - get_steam_id_data_from_login_user.Count;
            all_logs_detected_cleaning.Add($"Обнаружено {Math.Abs(account_detect)} удалённых аккаунтов");
        }
        public void search_all()
        {
            this.detect_clear_steam_account();
            this.detect_clear_protected_folder();
            if (all_logs_detected_cleaning.Count == 0)
                return;
            string message = string.Empty;
            all_logs_detected_cleaning.ForEach((messages) => message += $"{messages}\n");
            MessageBox.Show(message);
        }
    }
}
