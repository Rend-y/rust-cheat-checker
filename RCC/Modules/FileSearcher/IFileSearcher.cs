using System;
using System.Collections.Generic;
using System.IO;

namespace RCC.Modules.FileSearcher;

public interface IFileSearcher<T>
{
    public List<T> FindFileList { get; }

    public void Run(
        DirectoryInfo directoryInfo,
        DateTime dateTimeStart,
        DateTime dateTimeEnd,
        string searchPattern,
        double minFileSize,
        double maxFileSize
    );
}