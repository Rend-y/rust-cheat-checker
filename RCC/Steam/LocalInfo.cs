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
            catch (Exception exception) { new MessageBox(exception.Message).Show(); }
            return result;
        }
        public static readonly string GetPathToLoginUser = $"{GetSteamLocation()}\\config\\loginusers.vdf";
        public static readonly string GetPathToConfig = $"{GetSteamLocation()}\\config\\config.vdf";
        public static List<string> GetSteamIsFromContent(string contentData) => Regex.Matches(contentData, "\\\"7656(.*?)\\\"").Cast<Match>().Select(x => "7656" + x.Groups[1].Value).ToList();

        public static List<string> GetSteamIdFromCoPlay()
        {
            List<string> files = Directory.GetFiles($"{GetSteamLocation()}\\config\\", "*.vdf")
                .Where(path => Regex.IsMatch(path, "\\d{17}")).Select(Path.GetFileName).ToList();
            var result = files.ConvertAll(item => Regex.Matches(item, @"_7656(.*?).vdf").Cast<Match>().Select(x => "7656" + x.Groups[1].Value).ToList()[0]);
            return result;
        }

        public static SteamData PeParseFromSteam(long steamId, bool isDeleted)
        {
            string url = $"https://steamcommunity.com/profiles/{steamId}";
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
            catch (Exception exception) { new MessageBox(exception.Message).Show(); };
            return new SteamData(username, steamId, level, avatarUrl, isHideProfile, isDeleted);

        }

        public static SteamData GetLastAccountInfo()
        {
            string steamPathToLoginUser = GetPathToLoginUser;
            string steamPathToConfig = GetPathToConfig;
            
            if (!File.Exists(steamPathToLoginUser) && !File.Exists(steamPathToConfig))
                return null;

            
            string loginUserFileData = File.ReadAllText(steamPathToLoginUser);
            string configFileData = File.ReadAllText(steamPathToConfig);
            List<string> getLoginUserSteamId = GetSteamIsFromContent(loginUserFileData);
            List<string> getConfigSteamId = GetSteamIsFromContent(configFileData);
            List<string> getCoPlaySteamId = GetSteamIdFromCoPlay();
            List<string> isLastAccount = Regex.Matches(loginUserFileData, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>().Select(x => x.Groups[1].Value).ToList();

            List<string> isDeletedAccount = new List<string>();
            List<string> normalAccount = new List<string>();
            normalAccount.AddRange(getLoginUserSteamId);
            normalAccount.AddRange(getLoginUserSteamId.Intersect(getConfigSteamId).ToList());
            isDeletedAccount.AddRange(getLoginUserSteamId.Except(getConfigSteamId).ToList());
            normalAccount.AddRange(getLoginUserSteamId.Intersect(getCoPlaySteamId).ToList());
            isDeletedAccount.AddRange(getLoginUserSteamId.Except(getCoPlaySteamId).ToList());
            isDeletedAccount = new HashSet<string>(isDeletedAccount).ToList();
            normalAccount = new HashSet<string>(normalAccount).ToList();

            for (int i = 0; i < isLastAccount.Count; i++)
            {
                if (isLastAccount[i] == true.ToString())
                    return PeParseFromSteam(long.Parse(getLoginUserSteamId[i]), false);
            }
            return PeParseFromSteam(long.Parse(getLoginUserSteamId[0]), false);
        }
    }
}
