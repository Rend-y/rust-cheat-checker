using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RCC.windows
{
    [Flags]
    public enum NotifyTypes
    {
        [Description("")]
        Warning,
        [Description("")]
        Error,
        [Description("")]
        Success,
        [Description("")]
        Any,
    }
    public partial class Notify : ICustomWindow<NotifyTypes>
    {
        public Notify()
        {
            InitializeComponent();
            this.LabelMessage.Text = null;
            glass_effect.enable_blur(this);
        }
        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            double screenHeight = SystemParameters.FullPrimaryScreenHeight;
            double screenWidth = SystemParameters.FullPrimaryScreenWidth;
            for (int i = 0; i <= 100; i++)
            {
                this.Top = screenHeight - this.Height;
                this.Left = screenWidth - this.Width + (this.Width / 100 * i) - 10;
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
            this.Close();
        }
        public void Dispose()
        {
            this.Close();
        }

        public async void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.LabelMessage.Text == null)
                this.LabelMessage.Visibility = Visibility.Collapsed;
            // make me this window on lock button position
            double screenHeight = SystemParameters.FullPrimaryScreenHeight;
            double screenWidth = SystemParameters.FullPrimaryScreenWidth;
            for (int i = 100; i >= 0; i--)
            {
                this.Top = screenHeight - this.Height;
                this.Left = screenWidth - this.Width + (this.Width / 100 * i) - 10;
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0,0,0,3);
            dispatcherTimer.Start();
            this.Topmost = true;  
        }
        public void Show(string title, string message = null, NotifyTypes messageType = NotifyTypes.Any)
        {
            using var notify = new Notify();
            notify.LabelTitle.Content = title;
            if (message != null)
                notify.LabelMessage.Text = message;
            notify.ShowDialog();
        }
    }
}