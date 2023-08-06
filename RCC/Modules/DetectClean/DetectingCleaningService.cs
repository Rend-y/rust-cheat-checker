using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RCC.Modules.SteamInformation;
using RCC.windows;

namespace RCC.Modules.DetectClean;

public class DetectingCleaningService : IDetectingCleaning<SDetectCleanData>
{
    private readonly ISteamInformation<SteamData> _steamInformation;

    public DetectingCleaningService(ISteamInformation<SteamData> steamInformation)
    {
        _steamInformation = steamInformation;
    }

    public List<string> AllLogsDetectedCleaning { get; } = new();
    public List<SDetectCleanData> FolderAndFileForClear { get; } = new();

    public List<string> DetectClearInFolder()
    {
        var result = new List<string>();
        foreach (var folder in FolderAndFileForClear)
            try
            {
                var directoryInfo = new DirectoryInfo(folder.FullPath);
                FileInfo[] files = directoryInfo.GetFiles();
                var filesCount = files.Length;
                if (filesCount <= 15)
                {
                    result.Add($"Обнаружена очищенная папка {folder.FolderName}");
                    result.Add($"Текушие количество файлов в папке {folder.FolderName}: {filesCount}");
                    continue;
                }

                var dateTime = DateTime.Now;
                files.ToList().ForEach(file =>
                {
                    if ((dateTime - file.CreationTime).TotalMinutes > 0.0)
                        dateTime = file.CreationTime;
                });
                var totalMinutes = (int)(DateTime.Now - dateTime).TotalMinutes;
                if (totalMinutes < 45.0)
                    result.Add($"Обнаружена попытка очистки папки {folder.FolderName}");
                else if (totalMinutes < 360)
                    result.Add($"Самый первый файл в {folder.FolderName} был создан {totalMinutes} минут назад");
            }
            catch (UnauthorizedAccessException)
            {
                new MessageBox(
                        $"Не удалось получить доступ к папке {folder.FolderName}\nПерезапустите программу от имени администратора")
                    .Show();
            }
            catch (Exception e)
            {
                new MessageBox($"Неизвестная ошибка\n" +
                               $"При попытке получить доступ к папке {folder.FolderName}\n" +
                               "Пожалуйста сообщите об этой ошибке разработчику\n" +
                               "Приложите скриншот этого окна\n" +
                               $"Сообщение об ошибке:\n{e.Message}\n").Show();
            }

        return result;
    }

    public List<string> DetectClearSteamAccount()
    {
        var result = new List<string>();
        var fileDataFromConfig = File.ReadAllText(_steamInformation.PathToSteamConfig);
        var fileDataFromLoginUser = File.ReadAllText(_steamInformation.PathToLoginData);
        var getSteamIdDataFromConfig = _steamInformation.GetSteamIdFromContent(fileDataFromConfig);
        var getSteamIdDataFromLoginUser = _steamInformation.GetSteamIdFromContent(fileDataFromLoginUser);
        var getSteamIdFromCoPlay = _steamInformation.GetSteamIdFromCoPlayData();
        var calculate = getSteamIdDataFromConfig.Count - getSteamIdDataFromLoginUser.Count -
                        getSteamIdFromCoPlay.Count;
        if (getSteamIdDataFromConfig.Count == Math.Abs(calculate))
            return result;
        result.Add("Обнаружены удаленные аккаунты");
        return result;
    }
}