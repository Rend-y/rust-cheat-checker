using System.IO;
using System.Threading;
using System.Windows;
using System.Security.Principal;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
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
                MessageBox.Show("Please run it's programm from admin");
                Environment.Exit(Environment.ExitCode);
            }
            Thread thread = new Thread(() =>
            {
                DetectingCleaning detecting = new DetectingCleaning();
                detecting.search_all();
            });
            Thread font = new Thread(() =>
            {
                list_fonts.ForEach((fonts) => new Thread(() =>
                    {
                        File.WriteAllBytes(fonts.Item1, fonts.Item2);
                        //File.Move(fonts.Item1, $"C:/Windows/Fonts/{fonts.Item1}");
                    }).Start());
            });
            MainWindow main = new MainWindow();
            thread.Start();
            font.Start();
            main.Show();
        }
    }
}
