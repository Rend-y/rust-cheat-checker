using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace RCC.Steam
{
    public class LocalInfo
    {
        public static string get_steam_location()
        {
            string steam_path_x64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
            string steam_path_x32 = @"Software\Valve\Steam";
            string result = string.Empty;
            try
            {
                bool is_x64_opearation_sysytem = Environment.Is64BitOperatingSystem;
                RegistryKey get_base_reg_dir = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, is_x64_opearation_sysytem ? RegistryView.Registry64 : RegistryView.Registry32);
                result = get_base_reg_dir.OpenSubKey(is_x64_opearation_sysytem ? steam_path_x64 : steam_path_x32, is_x64_opearation_sysytem)
                    .GetValue(is_x64_opearation_sysytem ? "InstallPath" : "SourceModInstallPath")?.ToString();
            }
            catch (Exception exept) { MessageBox.Show(exept.Message.ToString()); }
            return result;
        }

        private static string get_path_to_login() => $"{get_steam_location()}\\config\\loginusers.vdf";

        private static List<string> get_all_steam_id(string file_data) => Regex.Matches(file_data, "\\\"76(.*?)\\\"").Cast<Match>().Select(x => "76" + x.Groups[1].Value).ToList();

        public static string get_steam_id()
        {
            string result = string.Empty;
            string steam_path_to_login_user = get_path_to_login();

            if (!File.Exists(steam_path_to_login_user))
                return string.Empty;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = get_all_steam_id(file_data);
            List<string> get_last_activity_data = Regex.Matches(file_data, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();

            for (int i = 0; i < get_last_activity_data.Count; i++)
            {
                if (get_last_activity_data[i] == true.ToString())
                    result = get_steam_id_data[i];
            }

            if (result == string.Empty)
                result = get_steam_id_data[0];

            return result;
        }

        public static List<string> get_steam_all_steam_account()
        {
            string steam_path_to_login_user = get_path_to_login();

            if (!File.Exists(steam_path_to_login_user))
                return null;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            return get_all_steam_id(file_data);
        }
    }
}
