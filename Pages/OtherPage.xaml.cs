using System.Windows.Controls;
using System.Windows.Input;
using RCC.QuickCheck;

namespace RCC.Pages
{
    public partial class OtherPage : Page
    {
        public OtherPage()
        {
            InitializeComponent();
            
            DangerousApps.start_scan().ForEach(item => ListAllDangerousApps.Items.Add(item));
        }

        private void ButtonStartKeyBoardSearch_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => new KeyboardCheck();

        private void ButtonStartConsoleCommandSearch_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            new ConsoleCommand();
    }
}