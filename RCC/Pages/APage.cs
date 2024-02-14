using System.Threading;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;

namespace RCC.Pages;

public class APage : Page
{
    protected readonly ILogger Logger;
    private int _backgroundWorkerPercentProgress;
    private Thread? _workerThread;
    protected APage(ILogger logger, params object[] objects)
    {
        Logger = logger;
        Logger.LogInformation("Page loaded successfully");
    }
    protected void RunBackgroundWorker()
    {
        Logger.LogInformation("Page run worker async");
        _workerThread = new Thread(BackgroundWorkerDoWork);
        _workerThread.SetApartmentState(ApartmentState.STA);
        _workerThread.Start();
    }
    protected void BackgroundWorkerSendProgress(object obj)
    {
        Logger.LogInformation("Page background worker send progress {Progress}, {Obj}", _backgroundWorkerPercentProgress, obj);
        // _backgroundWorker.ReportProgress(_backgroundWorkerPercentProgress, obj);
        Dispatcher.Invoke(() => BackgroundWorkerProgressChanged(obj));
        _backgroundWorkerPercentProgress++;
    }

    protected virtual void BackgroundWorkerDoWork()
    {
        Logger.LogInformation("Page background worker start do work");
    }

    protected virtual void BackgroundWorkerProgressChanged(object sender)
    {
        Logger.LogInformation("Page background worker get progress {Obj}", sender);
    }
}