using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RCC.Components
{
    public partial class Button : UserControl
    {
        public Button()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public string Text { get; set; }
    }
}
