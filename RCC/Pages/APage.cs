using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;

namespace RCC.Pages;

public class APage : Page
{
    private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
    protected readonly ILogger Logger;
    private int _backgroundWorkerPercentProgress;
    protected APage(ILogger logger, params object[] objects)
    {
        Logger = logger;
        Logger.LogInformation("Page loaded successfully");
    }
    protected void RunBackgroundWorker()
    {
        Logger.LogInformation("Page run worker async");
        _backgroundWorker.DoWork += BackgroundWorkerDoWork;
        _backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
        _backgroundWorker.WorkerReportsProgress = true;
        _backgroundWorker.RunWorkerAsync();
    }
    protected void BackgroundWorkerSendProgress(object obj)
    {
        Logger.LogInformation("Page background worker send progress {Progress}, {Obj}", _backgroundWorkerPercentProgress, obj);
        _backgroundWorker.ReportProgress(_backgroundWorkerPercentProgress, obj);
        _backgroundWorkerPercentProgress++;
    }

    protected virtual void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
    {
        Logger.LogInformation("Page background worker start do work");
    }

    protected virtual void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        Logger.LogInformation("Page background worker get progress {Progress}, {Obj}", e.ProgressPercentage, e.UserState);
    }
}