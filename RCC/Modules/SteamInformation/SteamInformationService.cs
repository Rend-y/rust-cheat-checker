using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace RCC.Modules.SteamInformation;

public class SteamInformationService : ISteamInformation<SteamData>
{
    public SteamInformationService()
    {
        var getSteamLocation = GetSteamLocation();
        PathToLoginData = $"{getSteamLocation}\\config\\loginusers.vdf";
        PathToSteamConfig = $"{getSteamLocation}\\config\\config.vdf";
        PathToSteamConfigFolder = $"{getSteamLocation}\\config\\";
    }
    public string PathToSteamConfigFolder { get; }

    public string GetSteamLocation()
    {
        const string steamPathX64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
        const string steamPathX32 = @"Software\Valve\Steam";

        try
        {
            bool isX64OperationSystem = Environment.Is64BitOperatingSystem;
            using var baseRegDir = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                isX64OperationSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            using var subKey =
                baseRegDir.OpenSubKey(isX64OperationSystem ? steamPathX64 : steamPathX32, isX64OperationSystem)
                ?? throw new Exception("Steam sub key of registry not found");

            var valueName = isX64OperationSystem ? "InstallPath" : "SourceModInstallPath";
            return subKey.GetValue(valueName)?.ToString() ??
                   throw new Exception($"Steam key '{valueName}' of registry doesn't contain a value");
        }
        catch (Exception exception)
        {
            Debug.Fail(exception.Message, exception.StackTrace);
            return string.Empty;
        }
    }

    public List<string> GetSteamIdFromContent(in string content)
    {
        const string steamIdPrefix = "7656";
        var steamIdsHashSet = new HashSet<string>();

        if (string.IsNullOrEmpty(content))
            return new List<string>();

        for (var startIndex = 0; startIndex < content.Length;)
        {
            var prefixIndex = content.IndexOf(steamIdPrefix, startIndex, StringComparison.Ordinal);

            if (prefixIndex == -1)
                break;

            var endIndex = prefixIndex + steamIdPrefix.Length;

            while (endIndex < content.Length && char.IsDigit(content[endIndex]))
                endIndex++;

            if (endIndex > prefixIndex + steamIdPrefix.Length)
                steamIdsHashSet.Add(content.Substring(prefixIndex, endIndex - prefixIndex));

            startIndex = endIndex;
        }

        return steamIdsHashSet.ToList();
    }

    public SteamData GetSteamData(in long steamId, in bool isDeleted = false) => new SteamData(steamId, isDeleted);

    public SteamData GetLastSteamAccountInfo()
    {
        var loginUserDataPath = PathToLoginData;
        var configDataPath = PathToSteamConfig;

        if (!File.Exists(loginUserDataPath) && !File.Exists(configDataPath))
            throw new FileNotFoundException("Steam files were not found");

        var loginUserData = File.ReadAllText(loginUserDataPath);

        var loginUserSteamIds = GetSteamIdFromContent(loginUserData);

        var lastAccounts = loginUserData
            .Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("\"mostrecent\""))
            .Select(line => line.Split('\"')[3])
            .ToList();

        for (var i = 0; i < lastAccounts.Count; i++)
            if (bool.TryParse(lastAccounts[i], out var isLastAccount) && isLastAccount)
                if (long.TryParse(loginUserSteamIds.ElementAtOrDefault(i), out var steamId))
                    return GetSteamData(steamId);

        if (long.TryParse(loginUserSteamIds.FirstOrDefault(), out var defaultSteamId))
            return GetSteamData(defaultSteamId);

        return null;
    }

    public string PathToLoginData { get; }
    public string PathToSteamConfig { get; }

    public List<string> GetSteamIdFromCoPlayData()
    {
        var extractedNumbers = Directory.GetFiles(PathToSteamConfigFolder, "coplay_*.vdf", SearchOption.TopDirectoryOnly)
            .Select(file =>
            {
                var startIndex = file.IndexOf("coplay_") + "coplay_".Length;
                var endIndex = file.LastIndexOf(".vdf");
                var extractedNumber = file.Substring(startIndex, endIndex - startIndex);
                return extractedNumber;
            })
            .ToList();
        return extractedNumbers;
    }
}