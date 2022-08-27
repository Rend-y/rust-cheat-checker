using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RCC
{
    public class DetectingCleaning
    {
        List<String> all_logs_detected_cleaning = new List<string>();
        public DetectingCleaning() { }
        public void detect_clear_prefetch()
        {
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Prefetch", "*.pf");
            if (files.Count() == 0)
            {
                all_logs_detected_cleaning.Add("Prefetch folder cleaning was discovered");
                return;
            }

            DateTime dateTime = DateTime.Now;
            string[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                DateTime creationTime = File.GetCreationTime(array[i]);
                if ((dateTime - creationTime).TotalMinutes > 0.0)
                    dateTime = creationTime;
            }

            if ((DateTime.Now - dateTime).TotalMinutes < 45.0)
                all_logs_detected_cleaning.Add("An attempt was found to clear the Prefetch folder");
            else all_logs_detected_cleaning.Add($"The oldst file was been created {(int)((DateTime.Now - dateTime).TotalMinutes)} min ago");
        }
        public void detect_clear_steam_account()
        {
            string file_data_from_config = File.ReadAllText(Steam.LocalInfo.get_path_to_config());
            List<string> get_steam_id_data_from_config = Steam.LocalInfo.get_all_steam_id(file_data_from_config);
            string file_data_from_login_user = File.ReadAllText(Steam.LocalInfo.get_path_to_login_user());
            List<string> get_steam_id_data_from_login_user = Steam.LocalInfo.get_all_steam_id(file_data_from_login_user);
            if (get_steam_id_data_from_config.Count == get_steam_id_data_from_login_user.Count)
                return;

            int account_detect = get_steam_id_data_from_config.Count - get_steam_id_data_from_login_user.Count;
            all_logs_detected_cleaning.Add($"Account deletion detected at {Math.Abs(account_detect)}");
        }
        public void search_all()
        {
            this.detect_clear_steam_account();
            this.detect_clear_prefetch();
            if (all_logs_detected_cleaning.Count == 0)
                return;
            string message = string.Empty;
            foreach (string messages in all_logs_detected_cleaning)
                message += $"{messages}\n";
            MessageBox.Show(message);
        }
    }
}
