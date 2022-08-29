using System.IO;
using System.Threading;
using System.Windows;
using System.Security.Principal;
using System.Collections.Generic;
using System;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        List<Tuple<string, byte[]>> list_fonts = new List<Tuple<string, byte[]>>()
        {
            Tuple.Create(@"Font Awesome 6 Brands-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Brands_Regular_400),
            Tuple.Create(@"Font Awesome 6 Free-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Regular_400),
            Tuple.Create(@"Font Awesome 6 Free-Solid-900.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Solid_900),
        };
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                System.Windows.Forms.MessageBox.Show("Please run it's programm from admin");
                Environment.Exit(Environment.ExitCode);
            }
            string get_file_version_from_git = new WebClient().DownloadString("https://raw.githubusercontent.com/Midoruya/rust-cheat-checker/main/version.ini");
            string get_file_version_from_assembly = Assembly.GetEntryAssembly().GetName().Version.ToString();
            if (get_file_version_from_git.Equals(get_file_version_from_assembly) == false)
            {
                DialogResult button_pressed = System.Windows.Forms.MessageBox.Show(
                       "Вышла новая версия вы желаете обновится ?",
                       "Обновление",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Information,
                       MessageBoxDefaultButton.Button1,
                       System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
                if (button_pressed == DialogResult.Yes)
                {
                    new WebClient().DownloadFile($"https://github.com/Midoruya/rust-cheat-checker/releases/download/{get_file_version_from_git}/RCC.exe", "Updated.exe");
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C timeout 5 & del RCC.exe & move Updated.exe RCC.exe & del Updated.exe & runas RCC.exe";
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.CreateNoWindow = true;
                    Process proc = Process.Start(startInfo);
                    Environment.Exit(Environment.ExitCode);
                }
            }
            Thread thread = new Thread(() =>
            {
                DetectingCleaning detecting = new DetectingCleaning();
                detecting.search_all();
            });
            Thread font_thread = new Thread(() => list_fonts.ForEach((fonts) => new Thread(() =>
            {
                try
                {
                    string font_path = $"C:\\Windows\\Temp\\{fonts.Item1}";
                    File.WriteAllBytes(font_path, fonts.Item2);
                    AllDllImport.AddFontResource(font_path);
                }
                catch (Exception exept) { /* The current file uses another process */ }
            }).Start()));
            MainWindow main = new MainWindow();
            thread.Start();
            font_thread.Start();
            main.Show();
        }
    }
}
