using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace RCC.Pages
{
    public partial class GreetingPage : Page
    {
        public GreetingPage()
        {
            InitializeComponent();
        }

        private void ButtonToRedirectOnGit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => 
            Process.Start("https://github.com/Midoruya/rust-cheat-checker/releases/latest/");
    }
}