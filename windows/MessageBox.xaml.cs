using System;
using System.Windows;
using System.Windows.Input;

namespace RCC.windows
{
    public partial class MessageBox : IDisposable
    {
        private MessageBoxResult Result { get; set; }
        public MessageBox()
        {
            InitializeComponent();
            Result = MessageBoxResult.Cancel;
        }
        public void Dispose()
        {
            this.Close();
        }
        public static void Show(string message, string title = "Rust Cheat Checker")
        {
            using (var msg = new MessageBox())
            {
                msg.LabelTitle.Content = title;
                msg.TextBlockMessage.Text = message;

                msg.ButtonOk.Focus();
                msg.ShowDialog();
            }
        }
        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void MessageBox_OnLoaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);
        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }
    }
}