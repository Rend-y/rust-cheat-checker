using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using RCC.windows;

namespace RCC.Modules.SteamInformation;

public class SteamInformationService : ISteamInformation<SteamData>
{
    public SteamInformationService()
    {
        PathToLoginData = $"{GetSteamLocation()}\\config\\loginusers.vdf";
        PathToSteamConfig = $"{GetSteamLocation()}\\config\\config.vdf";
    }

    public string GetSteamLocation()
    {
        const string steamPathX64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
        const string steamPathX32 = @"Software\Valve\Steam";
        string result = string.Empty;
        try
        {
            bool isX64OperationSystem = Environment.Is64BitOperatingSystem;
            RegistryKey getBaseRegDir = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                isX64OperationSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            RegistryKey getSubKey =
                getBaseRegDir.OpenSubKey(isX64OperationSystem ? steamPathX64 : steamPathX32, isX64OperationSystem) ??
                throw new Exception("Steam sub key of registry not found");
            result = getSubKey.GetValue(isX64OperationSystem ? "InstallPath" : "SourceModInstallPath")?.ToString() ??
                     throw new Exception("Steam key of registry doesn't contains value");
        }
        catch (Exception exception)
        {
            new MessageBox(exception.Message).Show();
        }

        return result;
    }

    public List<string> GetSteamIdFromContent(string content) => Regex.Matches(content, "\\\"7656(.*?)\\\"")
        .Cast<Match>()
        .Select(x => "7656" + x.Groups[1].Value).ToList();

    public List<string> GetSteamIdFromCoPlayData()
    {
        List<string> files = Directory.GetFiles($"{GetSteamLocation()}\\config\\", "*.vdf")
            .Where(path => Regex.IsMatch(path, "\\d{17}")).Select(Path.GetFileName).ToList();
        var result = files.ConvertAll(item =>
            Regex.Matches(item, @"_7656(.*?).vdf").Cast<Match>().Select(x => "7656" + x.Groups[1].Value).ToList()[0]);
        return result;
    }

    public SteamData GetSteamData(in long steamId, in bool isDeleted = false) => new(steamId, isDeleted);

    public SteamData GetLastSteamAccountInfo()
    {
        string steamPathToLoginUser = PathToLoginData;
        string steamPathToConfig = PathToSteamConfig;

        if (!File.Exists(steamPathToLoginUser) && !File.Exists(steamPathToConfig))
            throw new FileNotFoundException("Steam files were not found");


        string loginUserFileData = File.ReadAllText(steamPathToLoginUser);
        string configFileData = File.ReadAllText(steamPathToConfig);
        List<string> getLoginUserSteamId = GetSteamIdFromContent(loginUserFileData);
        List<string> getConfigSteamId = GetSteamIdFromContent(configFileData);
        List<string> getCoPlaySteamId = GetSteamIdFromCoPlayData();
        List<string> isLastAccount = Regex.Matches(loginUserFileData, "\\\"mostrecent\\\"		\\\"(.*?)\\\"").Cast<Match>()
            .Select(x => x.Groups[1].Value).ToList();

        // List<string> isDeletedAccount = new List<string>();
        // List<string> normalAccount = new List<string>();
        // normalAccount.AddRange(getLoginUserSteamId);
        // normalAccount.AddRange(getLoginUserSteamId.Intersect(getConfigSteamId).ToList());
        // isDeletedAccount.AddRange(getLoginUserSteamId.Except(getConfigSteamId).ToList());
        // normalAccount.AddRange(getLoginUserSteamId.Intersect(getCoPlaySteamId).ToList());
        // isDeletedAccount.AddRange(getLoginUserSteamId.Except(getCoPlaySteamId).ToList());
        // isDeletedAccount = new HashSet<string>(isDeletedAccount).ToList();
        // normalAccount = new HashSet<string>(normalAccount).ToList();

        for (int i = 0; i < isLastAccount.Count; i++)
        {
            if (isLastAccount[i] == true.ToString())
                return GetSteamData(long.Parse(getLoginUserSteamId[i]));
        }

        return GetSteamData(long.Parse(getLoginUserSteamId[0]));
    }

    public string PathToLoginData { get; }

    public string PathToSteamConfig { get; }
}