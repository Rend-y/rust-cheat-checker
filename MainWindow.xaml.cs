using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace RCC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MessageBox.Show(GetSysthemInfo.get_screen_size);
            InitializeComponent();
        }
    }
}
