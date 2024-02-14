using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace RCC.Pages
{
    public partial class GreetingPage : APage
    {
        public GreetingPage(ILogger<GreetingPage> logger) : base(logger)
        {
            InitializeComponent();
        }

        private void ButtonToRedirectOnGit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            Process.Start("https://github.com/Midoruya/rust-cheat-checker/releases/latest/");
    }
}