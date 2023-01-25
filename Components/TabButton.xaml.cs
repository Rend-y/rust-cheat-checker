using System.Windows.Controls;

namespace RCC.Components
{
    public partial class TabButton : UserControl
    {
        public TabButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string Icon { get; set; }
        public string Text { get; set; }
    }
}