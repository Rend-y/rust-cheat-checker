using System.IO;
using System.Threading;
using System.Windows;
using System.Security.Principal;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                MessageBox.Show("Please run it's programm from admin");
                return;
            }
            Thread thread = new Thread(() =>
            {
                DetectingCleaning detecting = new DetectingCleaning();
                detecting.search_all();
            });
            Thread font = new Thread(() =>
            {
                File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Brands-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Brands_Regular_400);
                File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Free-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Regular_400);
                File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Free-Solid-900.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Solid_900);
                AllDllImport.AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Brands-Regular-400.otf");
                AllDllImport.AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Free-Regular-400.otf");
                AllDllImport.AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Free-Solid-900.otf");
            });
            MainWindow main = new MainWindow();
            thread.Start();
            font.Start();
            main.Show();
        }
    }
}
