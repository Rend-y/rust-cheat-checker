using System;
using System.Windows;

namespace RCC.windows
{
    public partial class Notify : IDisposable
    {
        public Notify()
        {
            InitializeComponent();
            this.LabelMessage.Text = null;
        }
        public void Dispose()
        {
            this.Close();
        }
        private void Notify_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.LabelMessage.Text == null)
                this.LabelMessage.Visibility = Visibility.Collapsed;
            glass_effect.enable_blur(this);
        }
        public static void Show(string title, string message = null)
        {
            using (var notify = new Notify())
            {
                notify.LabelTitle.Content = title;
                if (message != null)
                    notify.LabelMessage.Text = message;
                notify.ShowDialog();
            }
        }
    }
}