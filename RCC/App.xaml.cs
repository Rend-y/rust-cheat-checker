﻿using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;
using RCC.Modules.DangerousApp;
using RCC.Modules.DetectClean;
using RCC.Modules.SteamInformation;
using RCC.Pages;
using RCC.windows;

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
                    collection.AddSingleton<ISteamInformation<SteamData>, SteamInformationService>();
                    collection.AddSingleton<IDetectingCleaning<SDetectCleanData>, DetectingCleaningService>();
                    collection.AddSingleton<IDangerousApp<SDangerousApplication>, DangerousAppService>();
                    collection.AddSingleton<MainWindow>();
                    collection.AddSingleton<GreetingPage>();
                    collection.AddSingleton<LastActivityPage>();
                    collection.AddSingleton<MouseLoggerPage>();
                    collection.AddSingleton<OtherPage>();
                    collection.AddSingleton<SearchFilePage>();
                    collection.AddSingleton<SteamDataPage>();
                    collection.AddSingleton<UsbDevicePage>();
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
                new MessageBox().Show("Please run it's program from admin");
                Environment.Exit(Environment.ExitCode);
            } 
            Utilities.CheckOnUpdate();
            Utilities.OpenDiscordServer();
#endif

            // AppHost.Services.GetService<Notify>()!.Show();
            new Notify("title", "message").Show();
            // DetectingCleaning.Start();
            AppHost.Services.GetService<MainWindow>()?.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}