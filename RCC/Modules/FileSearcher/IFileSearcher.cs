using System;
using System.Collections.Generic;
using System.IO;

namespace RCC.Modules.FileSearcher;

public interface IFileSearcher<T>
{
    public DirectoryInfo DirectoryInfo { get; }
    public DateTime DateTimeStart { get; }
    public DateTime DateTimeEnd { get; }
    public string SearchPattern { get; }
    public double MinFileSize { get; }
    public double MaxFileSize { get; }
    public List<T> FindFileList { get; }
    List<T> SearchFile(in DirectoryInfo directoryInfo);
    void Run();
}