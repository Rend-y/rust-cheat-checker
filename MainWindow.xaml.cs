using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using RCC.Pages;
using Path = System.IO.Path;
using ThreadState = System.Threading.ThreadState;

namespace RCC
{
    public partial class main_window : Window
    {
        private void window_loaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);
        private readonly SteamDataPage steamDataPage = new SteamDataPage();
        private readonly MouseLoggerPage mouseLoggerPage = new MouseLoggerPage();
        private readonly LastActivityPage lastActivityPage = new LastActivityPage();
        private readonly UsbDevicePage usbDevicesPage = new UsbDevicePage();
        private readonly SearchFilePage searchFilePage = new SearchFilePage();
        public main_window()
        {
            InitializeComponent();

            WindowsPageManager(new GreetingPage());

            DangerousApps.start_scan().ForEach(item => list_all_dangerous_apps.Items.Add(item));
        }
        private void WindowsPageManager(Page newPage) => PagesFrame.Content = newPage; 
        private void button_show_account_info_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(steamDataPage);
        private void button_show_usb_device_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(usbDevicesPage);
        private void button_show_last_activity_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(lastActivityPage);
        private void button_show_mouse_check_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(mouseLoggerPage);
        private void Button_show_search_file_OnMouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(searchFilePage);
        private void button_show_other_page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(new SteamDataPage());
        private void grid_custom_title_bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void label_close_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Environment.Exit(Environment.ExitCode);
        private void label_turn_off_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;
    }
}
