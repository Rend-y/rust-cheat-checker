using System.Windows.Controls;

namespace RCC
{
    public partial class CustomTabButtonControl : UserControl
    {
        public CustomTabButtonControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        
        public string Icon { get; set; }
        public string Text { get; set; }
    }
}