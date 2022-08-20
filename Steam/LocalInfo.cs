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
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace RCC.Steam
{
    public class LocalInfo
    {
        public class SteamData
        {
            public string username;
            public long steam_id;
            public int account_level;
            public System.Windows.Controls.Image avatar;

            public SteamData(string username,long stem_id,int accoung_level, string avatar_url)
            {
                this.username = username;
                this.steam_id = stem_id;
                this.account_level = accoung_level;
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(avatar_url);
                Bitmap bitmap;
                bitmap = new Bitmap(stream);
                stream.Close();
                image.Source = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                this.avatar = image;
            }

            public System.Windows.Controls.Image get_account_avatar => this.avatar;
            public string get_account_level => $"Accoung level : {this.account_level}";
            public string get_username => $"Username : {this.username}";
            public string get_steam_id => $"Steam Id : {this.steam_id}";
        }

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

        public static SteamData parse_from_steam(long steam_id)
        {
            string url = $"https://steamcommunity.com/profiles/{steam_id}";
            using (WebClient client = new WebClient())
            {
                //качаем страницу
                byte[] data = null;
                data = client.DownloadData(url);
                string text = Encoding.UTF8.GetString(data);

                string has_ban_in_line = @"<span class=""profile_ban_status"">([^"">]+)";
                var has_ban_in = Regex.Matches(text, has_ban_in_line).Cast<Match>().Select(x => x.Groups[1].Value).ToList();

                //парсим ник
                string nickname_line = @"<span class=""actual_persona_name"">([^"">]+)</span>";
                string username = Regex.Match(text, nickname_line).Groups[1].Value;

                //парсим уровень
                string level_line = @"<span class=""friendPlayerLevelNum"">([^"">]+)</span>";
                int level = int.Parse(Regex.Match(text, level_line).Groups[1].Value);

                //парсим адрес аватара и качаем аватар
                var avatar_url_array = Regex.Matches(text, @"<img src=""([^"">]+)"">").Cast<Match>().Select(x => x.Groups[1].Value).ToList();

                var avatar_url = Regex.Match(text, @"<div class=""playerAvatarAutoSizeInner"">(.*?)</div>").Groups[1].Value;
                foreach (string img in avatar_url_array)
                {
                    if (img.Contains("_full"))
                    {
                        avatar_url = img;
                        break;
                    }
                }


                return new SteamData(username, steam_id, level, avatar_url);
            }
        }

        public static SteamData get_last_account_info()
        {
            string steam_path_to_login_user = get_path_to_login();

            if (!File.Exists(steam_path_to_login_user))
                return null;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = get_all_steam_id(file_data);
            List<string> is_last_account = Regex.Matches(file_data, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            List<string> get_username = Regex.Matches(file_data, "\\\"PersonaName\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();

            for (int i = 0; i < is_last_account.Count; i++)
            {
                if (is_last_account[i] == true.ToString())
                    return parse_from_steam(long.Parse(get_steam_id_data[i]));
            }

            return parse_from_steam(long.Parse(get_steam_id_data[0]));
        }

        public static List<SteamData> get_steam_all_steam_account()
        {
            string steam_path_to_login_user = get_path_to_login();

            if (!File.Exists(steam_path_to_login_user))
                return null;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = get_all_steam_id(file_data);
            List<string> get_username = Regex.Matches(file_data, "\\\"PersonaName\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            List<SteamData> result = new List<SteamData>();

            for (int i = 0; i < get_steam_id_data.Count; i++)
                result.Add(parse_from_steam(long.Parse(get_steam_id_data[i])));

            return result;
        }
    }
}
