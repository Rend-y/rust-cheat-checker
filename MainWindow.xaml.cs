using RCC.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RCC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LocalInfo.get_steam_all_steam_account().ForEach(delegate (string id) { MessageBox.Show(id); });
            MessageBox.Show($"steam id to last active account {LocalInfo.get_steam_id()}");
            InitializeComponent();
        }
    }
}
