using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace RCC.Pages
{
    public partial class GreetingPage : Page
    {
        private readonly ILogger _logger;

        public GreetingPage(ILogger<GreetingPage> logger)
        {
            InitializeComponent();
            _logger = logger;
            _logger.LogInformation("Greeting page loaded successfully");
        }

        private void ButtonToRedirectOnGit_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            Process.Start("https://github.com/Midoruya/rust-cheat-checker/releases/latest/");
    }
}