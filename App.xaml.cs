using System.Threading;
using System.Windows;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using RCC.windows;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly List<Tuple<string, string>> list_fonts = new List<Tuple<string, string>>
        {
            Tuple.Create(@"Font Awesome 6 Brands-Regular-400.otf",
                "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/Font%20Awesome%206%20Brands-Regular-400.otf?raw=true"),
            Tuple.Create(@"Font Awesome 6 Free-Regular-400.otf",
                "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/Font%20Awesome%206%20Free-Regular-400.otf?raw=true"),
            Tuple.Create(@"Font Awesome 6 Free-Solid-900.otf",
                "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/Font%20Awesome%206%20Free-Solid-900.otf?raw=true"),
        };

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool isAdmin = Utilities.IsAdminStartup();
            if (!isAdmin)
            {
                System.Windows.Forms.MessageBox.Show("Please run it's programm from admin");
                Environment.Exit(Environment.ExitCode);
            }
            Utilities.CheckOnUpdate();
            list_fonts.ForEach(fonts => new Thread(() =>
            {
                try
                {
                    string fontPath = $"C:\\Windows\\Temp\\{fonts.Item1}";
                    bool isExistFont = File.Exists(fontPath);
                    if (!isExistFont)
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(fonts.Item2, fontPath);
                        }
                    }

                    AllDllImport.AddFontResource(fontPath);
                }
                catch (Exception exception)
                {
                    // ignored
                }
            }));
            // TODO: need uncommitted this before release
            //Utilities.OpenDiscordServer();
            DetectingCleaning.Start();
            Notify.Show("Приложение запущено", "Приложение Rust cheat checker\nуспешно запущено");
            main_window main = new main_window();
            main.Show();
        }
    }
}
