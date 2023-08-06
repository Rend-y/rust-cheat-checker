namespace RCC.Modules.DetectClean;

public struct SDetectCleanData
{
    public string FullPath { get; set; }
    public string SearchPattern { get; set; }
    public string FolderName { get; set; }

    public SDetectCleanData(string fullPath, string any, string folderName)
    {
        FullPath = fullPath;
        SearchPattern = any;
        FolderName = folderName;
    }
}