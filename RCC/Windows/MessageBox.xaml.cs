using System;
using System.Windows;
using System.Windows.Input;

namespace RCC.Windows
{
    public partial class MessageBox : ICustomWindow<EWindowsType>
    {
        public MessageBox(in string message, in string title = "Rust Cheat Checker",
            in EWindowsType messageType = EWindowsType.Any)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            InitializeComponent();
            Result = MessageBoxResult.None;
            LabelTitle.Content = title;
            TextBlockMessage.Text = message;
            WindowType = messageType;

            ButtonOk.Focus();
        }

        public MessageBoxResult Result { get; private set; }
        public EWindowsType WindowType { get; set; }
        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void MessageBox_OnLoaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void ButtonOk_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
    }
}