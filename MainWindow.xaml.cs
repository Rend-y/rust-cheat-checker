using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RCC.Pages;

namespace RCC
{
    public partial class main_window : Window
    {
        private void window_loaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);
        private readonly SteamDataPage _steamDataPage = new SteamDataPage();
        private readonly MouseLoggerPage _mouseLoggerPage = new MouseLoggerPage();
        private readonly LastActivityPage _lastActivityPage = new LastActivityPage();
        private readonly UsbDevicePage _usbDevicesPage = new UsbDevicePage();
        private readonly SearchFilePage _searchFilePage = new SearchFilePage();
        private readonly OtherPage _otherPage = new OtherPage();
        
        public main_window()
        {
            InitializeComponent();
            WindowsPageManager(new GreetingPage());
        }
        private void WindowsPageManager(Page newPage) => PagesFrame.Content = newPage; 
        private void button_show_account_info_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(_steamDataPage);
        private void button_show_usb_device_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(_usbDevicesPage);
        private void button_show_last_activity_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(_lastActivityPage);
        private void button_show_mouse_check_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(_mouseLoggerPage);
        private void Button_show_search_file_OnMouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(_searchFilePage);
        private void button_show_other_page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(_otherPage);
        private void grid_custom_title_bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void label_close_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Environment.Exit(Environment.ExitCode);
        private void label_turn_off_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;
    }
}
