using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
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
            public BitmapSource avatar;
            public bool is_hide_account;

            public SteamData(string username,long stem_id, int accoung_level, string avatar_url, bool is_hide_account)
            {
                this.username = username;
                this.steam_id = stem_id;
                this.account_level = accoung_level;
                WebClient client = new WebClient();
                Bitmap bitmap;
                if (avatar_url == string.Empty)
                    avatar_url = "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg";
                Stream stream = client.OpenRead(avatar_url);
                bitmap = new Bitmap(stream);
                stream.Close();
                this.avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); ;
                this.is_hide_account = is_hide_account;
            }

            public ImageSource get_account_avatar => this.avatar;
            public string get_account_level => this.account_level.ToString();
            public string get_username => $"Username : {this.username}";
            public string get_steam_id => $"Steam Id : {this.steam_id}";
            public Visibility get_is_hide_for_window => this.is_hide_account ? Visibility.Visible : Visibility.Hidden;
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
            string username = "", avatar_url = "";
            int level = 1;
            bool is_hide_profile = false;
            try
            {
                WebClient client = new WebClient();
                //качаем страницу
                byte[] data = null;
                data = client.DownloadData(url);
                string text = Encoding.UTF8.GetString(data);

                is_hide_profile = (bool)(Regex.Match(text, @"<div\s+class=""profile_private_info"">([^"">]+)</div>").Groups[1].Length > 0);
                
                //парсим ник
                string nickname_line = @"<span class=""actual_persona_name"">([^"">]+)</span>";
                username = Regex.Match(text, nickname_line).Groups[1].Value;

                //парсим уровень
                if (is_hide_profile) level = -1;
                else level = int.Parse(Regex.Match(text, @"<span class=""friendPlayerLevelNum"">([^"">]+)</span>").Groups[1].Value);

                //парсим адрес аватара и качаем аватар
                var avatar_url_array = Regex.Matches(text, @"<img src=""([^"">]+)"">").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
                foreach (string img in avatar_url_array)
                {
                    if (img.Contains("_full"))
                    {
                        avatar_url = img;
                        break;
                    }
                }
            }
            catch (Exception exept)
            {
                MessageBox.Show(exept.Message);
            }
            return new SteamData(username, steam_id, level, avatar_url, is_hide_profile);

        }

        public static SteamData get_last_account_info()
        {
            string steam_path_to_login_user = get_path_to_login();

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

        public static List<SteamData> get_steam_all_steam_account()
        {
            string steam_path_to_login_user = get_path_to_login();

            if (!File.Exists(steam_path_to_login_user))
                return null;

            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = get_all_steam_id(file_data);
            List<SteamData> result = new List<SteamData>();

            for (int i = 0; i < get_steam_id_data.Count; i++)
                result.Add(parse_from_steam(long.Parse(get_steam_id_data[i])));

            return result;
        }
    }
}
