using System.IO;
using System.Threading;
using System.Windows;

namespace RCC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void reg_font()
        {
            File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Brands-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Brands_Regular_400);
            File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Free-Regular-400.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Regular_400);
            File.WriteAllBytes(@"C:/Windows/Fonts/Font Awesome 6 Free-Solid-900.otf", RCC.Properties.Resources.Font_Awesome_6_Free_Solid_900);
            AllDllImport.AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Brands-Regular-400.otf");
            AllDllImport.AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Free-Regular-400.otf");
            AllDllImport.AddFontResourceA(@"C:/Windows/Fonts/Font Awesome 6 Free-Solid-900.otf");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DetectingCleaning detecting_cleaning = new DetectingCleaning();
            detecting_cleaning.search_all();
            Thread font = new Thread(reg_font);
            font.Start();
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
