using System.Collections.Generic;

namespace RCC.Modules.DetectClean;

public interface IDetectingCleaning<TData>
{
    List<string> AllLogsDetectedCleaning { get; }
    List<TData> FolderAndFileForClear { get; }
    List<string> DetectClearInFolder();
    List<string> DetectClearSteamAccount();
}