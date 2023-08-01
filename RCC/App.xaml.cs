using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;
using RCC.windows;
using MessageBox = RCC.windows.MessageBox;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? AppHost { get; set; }
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
                }).Build();
        }
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
            new Notify("title","message").Show();
            DetectingCleaning.Start();
            main_window main = new main_window();
            main.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
