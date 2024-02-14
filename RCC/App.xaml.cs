using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;
using RCC.Modules.AutoCheck.ConsoleCommand;
using RCC.Modules.AutoCheck.KeyboardCheck;
using RCC.Modules.DangerousApp;
using RCC.Modules.DetectClean;
using RCC.Modules.Device.KeyboardEvent;
using RCC.Modules.Device.MouseEvent;
using RCC.Modules.FileSearcher;
using RCC.Modules.Server;
using RCC.Modules.SteamInformation;
using RCC.Modules.SystemInfo;
using RCC.Pages;
using RCC.Windows;
using MessageBox = RCC.Windows.MessageBox;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, collection) =>
                {
                    collection.AddLogging(builder =>
                    {
                        builder.AddNLog();
                        builder.AddConsole();
                        builder.AddDebug();
                        builder.AddJsonConsole();
                        builder.AddConfiguration();
                        builder.SetMinimumLevel(LogLevel.Debug);
                    });

                    // Interface
                    collection.AddSingleton<MainWindow>();
                    collection.AddSingleton<GreetingPage>();
                    collection.AddSingleton<LastActivityPage>();
                    collection.AddSingleton<MouseLoggerPage>();
                    collection.AddSingleton<OtherPage>();
                    collection.AddSingleton<SearchFilePage>();
                    collection.AddSingleton<SteamDataPage>();
                    collection.AddSingleton<UsbDevicePage>();
                    // Modules
                    collection.AddSingleton<IServerService, ServerService>();
                    collection.AddSingleton<ISteamInformation<SteamData>, SteamInformationService>();
                    collection.AddSingleton<IDetectingCleaning<SDetectCleanData>, DetectingCleaningService>();
                    collection.AddSingleton<IDangerousApp<SDangerousApplication>, DangerousAppService>();
                    collection.AddSingleton<ISystemInfo, SystemInfoService>();
                    collection.AddSingleton<IConsoleCommand, ConsoleCommand>();
                    collection.AddSingleton<IKeyboardCheck, KeyboardCheck>();
                    collection.AddSingleton<IFileSearcher<FileInformation>, FileSearcher>();
                    // Utils
                    collection.AddSingleton<IKeyboardEvent<KeyboardKeys>, KeyboardEvent>();
                    collection.AddSingleton<IMouseEvent<MouseEventFlags>, MouseEvent>();
                    // Other
                }).Build();
        }
        private static IHost? AppHost { get; set; }
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await AppHost!.StartAsync();
#if !DEBUG
            bool isAdmin = Utilities.IsAdminStartup();
            if (!isAdmin)
            {
                new MessageBox("Please run it's program from admin").Show();
                Environment.Exit(Environment.ExitCode);
            }
#endif
            AppHost.Services.GetService<IServerService>()?.CheckOnUpdate();

            new Notify("title", "message").Show();
            AppHost.Services.GetService<MainWindow>()?.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}