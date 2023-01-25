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
        
        public string Width { get; set; }
        public string Height { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }

        private void Button_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.LabelIcon.Visibility = this.Text == "" ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            this.LabelText.Visibility = this.Icon == "" ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
        }
    }
}
