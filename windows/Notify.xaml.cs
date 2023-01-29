using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace RCC.windows
{
    public partial class Notify : IDisposable
    {
        public Notify()
        {
            InitializeComponent();
            //  DispatcherTimer setup
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0,0,5);
            dispatcherTimer.Start();
            this.LabelMessage.Text = null;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
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
            // make me this window on lock button position
            this.Topmost = true;    
            double screenHeight = SystemParameters.FullPrimaryScreenHeight;
            double screenWidth = SystemParameters.FullPrimaryScreenWidth;
 
            this.Top = (screenHeight - this.Height);
            this.Left = (screenWidth - this.Width) - 10;
        }
        public static void Show(string title, string message = null)
        {
            Thread thread = new Thread(() =>
            {
                using (var notify = new Notify())
                {
                    notify.LabelTitle.Content = title;
                    if (message != null)
                        notify.LabelMessage.Text = message;
                    notify.ShowDialog();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}