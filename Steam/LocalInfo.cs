using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RCC.Steam
{
    public class LocalInfo
    {
        /// <summary>
        /// Use this function for get path to folder (steam)
        /// </summary>
        /// <returns>full path to steam</returns>
        public static string get_steam_location()
        {
            string steam_path_x64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
            string steam_path_x32 = @"Software\Valve\Steam";
            string result = string.Empty;
            try
            {
                bool is_x64_operation_system = Environment.Is64BitOperatingSystem;
                RegistryKey get_base_reg_dir = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, is_x64_operation_system ? RegistryView.Registry64 : RegistryView.Registry32);
                result = get_base_reg_dir.OpenSubKey(is_x64_operation_system ? steam_path_x64 : steam_path_x32, is_x64_operation_system)
                    .GetValue(is_x64_operation_system ? "InstallPath" : "SourceModInstallPath")?.ToString();
            }
            catch (Exception exept) { MessageBox.Show(exept.Message.ToString()); }
            return result;
        }

        /// <summary>
        /// use this for get full path to file which keeps all accounts data
        /// </summary>
        /// <returns>full path to file which keeps all accounts data</returns>
        public static string get_path_to_login_user() => $"{get_steam_location()}\\config\\loginusers.vdf";
        public static string get_path_to_config() => $"{get_steam_location()}\\config\\config.vdf";

        public static List<string> get_all_steam_id(string file_data) => Regex.Matches(file_data, "\\\"7656(.*?)\\\"").Cast<Match>().Select(x => "7656" + x.Groups[1].Value).ToList();


        /// <summary>
        /// Use this to get steam data (avatar, username, is hide account, account level)
        /// if user account is hide. Then we return the level -1
        /// </summary>
        /// <param name="steam_id">steam id in steam</param>
        /// <returns>steam data (avatar, username, is hide account, account level)</returns>
        public static steam_data parse_from_steam(long steam_id)
        {
            string url = $"https://steamcommunity.com/profiles/{steam_id}";
            string username = "", avatar_url = "";
            int level = 1;
            bool is_hide_profile = false;
            try
            {
                WebClient client = new WebClient();
                byte[] data = null;
                data = client.DownloadData(url); // download full markup a frontend
                string text = Encoding.UTF8.GetString(data); // parse it's to utf 8 encode

                is_hide_profile = Regex.Match(text, @"<div\s+class=""profile_private_info"">([^"">]+)</div>").Groups[1].Length > 0; // parse is hide profile
                username = Regex.Match(text, @"<span class=""actual_persona_name"">([^"">]+)</span>").Groups[1].Value; // parse username
                if (is_hide_profile) level = -1;
                else level = int.Parse(Regex.Match(text, @"<span class=""friendPlayerLevelNum"">([^"">]+)</span>").Groups[1].Value); // parse level
                var avatar_url_array = Regex.Matches(text, @"<img src=""([^"">]+)"">").Cast<Match>().Select(x => x.Groups[1].Value).ToList(); // parse avatar
                foreach (string img in avatar_url_array)
                {
                    if (img.Contains("_full"))
                    {
                        avatar_url = img;
                        break;
                    }
                }
            }
            catch (Exception exception) { MessageBox.Show(exception.Message); };
            return new steam_data(username, steam_id, level, avatar_url, is_hide_profile);

        }

        // TODO: this function return incorrect value
        /// <summary>
        /// use this to get last account
        /// </summary>
        /// <returns>Steam Data for current user (avatar, username, is hide account, account level)</returns>
        public static steam_data get_last_account_info()
        {
            string steam_path_to_login_user = get_path_to_login_user();

            if (!File.Exists(steam_path_to_login_user))
                return null;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = get_all_steam_id(file_data);
            List<string> is_last_account = Regex.Matches(file_data, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            for (int i = 0; i < is_last_account.Count; i++)
            {
                if (is_last_account[i] == true.ToString())
                    return parse_from_steam(long.Parse(get_steam_id_data[i]));
            }

            return parse_from_steam(long.Parse(get_steam_id_data[0]));
        }

        /// <summary>
        /// use this ti get all array account
        /// </summary>
        /// <returns>array Steam Data (avatar, username, is hide account, account level)</returns>
        public static List<steam_data> get_steam_all_steam_account()
        {
            string steam_path_to_login_user = get_path_to_login_user();

            if (!File.Exists(steam_path_to_login_user))
                return null;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = get_all_steam_id(file_data);
            List<steam_data> result = new List<steam_data>();

            get_steam_id_data.ForEach(steam_id => result.Add(parse_from_steam(long.Parse(steam_id))));

            return result;
        }
    }
}
