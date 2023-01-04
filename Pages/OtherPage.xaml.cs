using System.Windows.Controls;

namespace RCC.Pages
{
    public partial class OtherPage : Page
    {
        public OtherPage()
        {
            InitializeComponent();
            
            DangerousApps.start_scan().ForEach(item => ListAllDangerousApps.Items.Add(item));
        }
    }
}