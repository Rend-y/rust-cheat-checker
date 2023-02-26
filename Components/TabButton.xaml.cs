using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using static RCC.Utilities;

namespace RCC.Components
{
    public enum Font: int
    {
        [Description("Font Awesome 6 Brands Regular")]
        BrandsRegular400,
        [Description("Font Awesome 6 Free Regular")]
        FreeRegular400,
        [Description("Font Awesome 6 Free Solid")]
        FreeSolid900,
    }
    public partial class TabButton : UserControl
    {
        public TabButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string FontIcon { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }
    }
}