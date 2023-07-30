using System;
using System.Threading.Tasks;
using System.Windows;

namespace RCC.windows
{
    public partial class Notify : ICustomWindow<EWindowsType>
    {
        public EWindowsType WindowType { get; set; }
        public Notify(in string title,in string message = "",in EWindowsType messageType = EWindowsType.Any)
        {
            if (title == null) throw new ArgumentNullException($"{nameof(title)} can't be nulleble");
            InitializeComponent();
            LabelMessage.Text = message;
            LabelTitle.Content = title;
            WindowType = messageType;
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
        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
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
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0,0,0,3);
            dispatcherTimer.Start();
            this.Topmost = true;
        }
    }
}