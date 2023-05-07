using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using RCC.windows;

namespace RCC.Steam
{
    public static class LocalInfo
    {
        /// <summary>
        /// Use this function for get path to folder (steam)
        /// </summary>
        /// <returns>full path to steam</returns>
        public static string GetSteamLocation()
        {
            const string steamPathX64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
            const string steamPathX32 = @"Software\Valve\Steam";
            string result = string.Empty;
            try
            {
                bool isX64OperationSystem = Environment.Is64BitOperatingSystem;
                RegistryKey getBaseRegDir = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, isX64OperationSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                result = getBaseRegDir.OpenSubKey(isX64OperationSystem ? steamPathX64 : steamPathX32, isX64OperationSystem)
                    ?.GetValue(isX64OperationSystem ? "InstallPath" : "SourceModInstallPath")?.ToString();
            }
            catch (Exception exception) { MessageBox.Show(exception.Message); }
            return result;
        }

        /// <summary>
        /// use this for get full path to file which keeps all accounts data
        /// </summary>
        /// <returns>full path to file which keeps all accounts data</returns>
        public static readonly string GetPathToLoginUser = $"{GetSteamLocation()}\\config\\loginusers.vdf";
        public static readonly string GetPathToConfig = $"{GetSteamLocation()}\\config\\config.vdf";
        public static List<string> GetSteamIsFromContent(string contentData) => Regex.Matches(contentData, "\\\"7656(.*?)\\\"").Cast<Match>().Select(x => "7656" + x.Groups[1].Value).ToList();

        /// <summary>
        /// Use this to get steam data (avatar, username, is hide account, account level)
        /// if user account is hide. Then we return the level -1
        /// </summary>
        /// <param name="steam_id">steam id in steam</param>
        /// <returns>steam data (avatar, username, is hide account, account level)</returns>
        public static steam_data PeParseFromSteam(long steam_id)
        {
            string url = $"https://steamcommunity.com/profiles/{steam_id}";
            string username = "", avatarUrl = "";
            int level = 1;
            bool isHideProfile = false;
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] data = null;
                    data = client.DownloadData(url); // download full markup a frontend
                    string text = Encoding.UTF8.GetString(data); // parse it's to utf 8 encode

                    isHideProfile = Regex.Match(text, @"<div\s+class=""profile_private_info"">([^"">]+)</div>").Groups[1].Length > 0; // parse is hide profile
                    username = Regex.Match(text, @"<span class=""actual_persona_name"">([^"">]+)</span>").Groups[1].Value; // parse username
                    if (isHideProfile) level = -1;
                    else level = int.Parse(Regex.Match(text, @"<span class=""friendPlayerLevelNum"">([^"">]+)</span>").Groups[1].Value); // parse level
                    var avatarUrlArray = Regex.Matches(text, @"<img src=""([^"">]+)"">").Cast<Match>().Select(x => x.Groups[1].Value).ToList(); // parse avatar
                    foreach (string img in avatarUrlArray)
                    {
                        if (img.Contains("_full"))
                        {
                            avatarUrl = img;
                            break;
                        }
                    }
                    
                }
            }
            catch (Exception exception) { MessageBox.Show(exception.Message); };
            return new steam_data(username, steam_id, level, avatarUrl, isHideProfile);

        }

        // TODO: this function return incorrect value
        /// <summary>
        /// use this to get last account
        /// </summary>
        /// <returns>Steam Data for current user (avatar, username, is hide account, account level)</returns>
        public static steam_data GetLastAccountInfo()
        {
            string steamPathToLoginUser = GetPathToLoginUser;

            if (!File.Exists(steamPathToLoginUser))
                return null;

            string fileData = File.ReadAllText(steamPathToLoginUser);
            List<string> getSteamIdData = GetSteamIsFromContent(fileData);
            List<string> isLastAccount = Regex.Matches(fileData, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            for (int i = 0; i < isLastAccount.Count; i++)
            {
                if (isLastAccount[i] == true.ToString())
                    return PeParseFromSteam(long.Parse(getSteamIdData[i]));
            }
            return PeParseFromSteam(long.Parse(getSteamIdData[0]));
        }
    }
}
