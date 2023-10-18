using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

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
    public FileSearcher(DirectoryInfo directoryInfo, DateTime dateTimeStart, DateTime dateTimeEnd, string searchPattern,
        double minFileSize, double maxFileSize)
    {
        DirectoryInfo = directoryInfo;
        DateTimeStart = dateTimeStart;
        DateTimeEnd = dateTimeEnd;
        SearchPattern = searchPattern;
        MinFileSize = minFileSize;
        MaxFileSize = maxFileSize;
    }

    public DirectoryInfo DirectoryInfo { get; }
    public DateTime DateTimeStart { get; }
    public DateTime DateTimeEnd { get; }
    public string SearchPattern { get; }
    public double MinFileSize { get; }
    public double MaxFileSize { get; }
    public List<FileInformation> FindFileList { get; } = new();

    public List<FileInformation> SearchFile(in DirectoryInfo directoryInfo)
    {
        var result = new List<FileInformation>();
        directoryInfo.GetFiles(SearchPattern, SearchOption.TopDirectoryOnly).ToList().ForEach(
            file => // get all files at directory
            {
                try
                {
                    if (file.CreationTime.Date > DateTimeEnd ||
                        file.CreationTime.Date < DateTimeStart) return;
                    if ((double)file.Length / 1000 > MaxFileSize ||
                        (double)file.Length / 1000 < MinFileSize) return;

                    try
                    {
                        result.Add(new FileInformation(
                                file.Name,
                                file.CreationTime.ToString(CultureInfo.InvariantCulture),
                                file.DirectoryName ?? "Directory error",
                                (file.Length / 1024).ToString()
                            )
                        );
                    } // adding to result array new file
                    catch (Exception exception)
                    {
                        Debug.Fail(exception.Message, exception.StackTrace);
                        Thread.Sleep(1);
                    } // if this file gives an error
                }
                catch (Exception exception)
                {
                    Debug.Fail(exception.Message, exception.StackTrace);
                    Thread.Sleep(1);
                } // if this file gives an error.
            });

        return result;
    }

    public void Run()
    {
        try
        {
            List<DirectoryInfo> getAllDirectoryInfos = DirectoryInfo.GetDirectories().ToList();
            if (getAllDirectoryInfos.Count != 0)
                getAllDirectoryInfos.ForEach(directory => // For each directory, we launch a new search
                {
                    var second = new FileSearcher(directory, DateTimeStart, DateTimeEnd, SearchPattern,
                        MinFileSize, MaxFileSize);
                    second.Run(); // Start search in subdirectory
                    FindFileList.AddRange(second.FindFileList); // Get list of all files which statisfy all conditions
                });

            FindFileList.AddRange(SearchFile(DirectoryInfo)); // Find all files in current directory
        }
        catch (Exception exception)
        {
            Debug.Fail(exception.Message, exception.StackTrace);
        }
    }
}