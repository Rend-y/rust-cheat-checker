using RCC.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);
        private void reg_font()
        {
            File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Brands-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Brands_Regular_400);
            File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Free-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Regular_400);
            File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Free-Solid-900.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Solid_900);
            AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Brands-Regular-400.otf");
            AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Free-Regular-400.otf");
            AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Free-Solid-900.otf");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Thread font = new Thread(reg_font);
            font.Start();
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
