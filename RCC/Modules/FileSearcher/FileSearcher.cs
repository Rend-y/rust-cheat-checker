using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RCC.Modules.FileSearcher;

public class FileInformation
{
    public FileInformation(string fileName, string createDate, string directory, string size)
    {
        CreateDate = createDate;
        Directory = directory;
        Size = size;
        FileName = fileName;
    }

    public string FileName { get; }
    public string CreateDate { get; }
    public string Directory { get; }
    public string Size { get; }
}

public class FileSearcher : IFileSearcher<FileInformation>
{
    public List<FileInformation> FindFileList { get; } = new();

    public void Run(DirectoryInfo directoryInfo, DateTime dateTimeStart, DateTime dateTimeEnd, string searchPattern,
        double minFileSize, double maxFileSize)
    {
        try
        {
            var workDirectoryInfo = directoryInfo;
            List<DirectoryInfo> allDirectory = workDirectoryInfo.GetDirectories().ToList();
            allDirectory.ForEach(directory =>
            {
                var filesList = directory.GetFiles(searchPattern, SearchOption.AllDirectories)
                    .Where(file =>
                    {
                        if (file.CreationTime.Date > dateTimeEnd ||
                            file.CreationTime.Date < dateTimeStart) return false;
                        if ((double)file.Length / 1000 > maxFileSize ||
                            (double)file.Length / 1000 < minFileSize) return false;
                        return true;
                    })
                    .Select<FileInfo, FileInformation>(file => new FileInformation(
                        file.Name,
                        file.CreationTime.ToString(CultureInfo.InvariantCulture),
                        file.DirectoryName ?? "Directory not found",
                        (file.Length / 1024).ToString()))
                    .ToList();
                FindFileList.AddRange(filesList);
            });
        }
        catch (Exception exception)
        {
            // Debug.Fail(exception.Message, exception.StackTrace);
        }
    }
}