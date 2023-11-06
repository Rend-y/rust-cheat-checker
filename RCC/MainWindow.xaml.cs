using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using RCC.Pages;

namespace RCC
{
    public partial class MainWindow : Window
    {
        private readonly LastActivityPage _lastActivityPage;
        private readonly ILogger _logger;
        private readonly MouseLoggerPage _mouseLoggerPage;
        private readonly OtherPage _otherPage;
        private readonly SearchFilePage _searchFilePage;
        private readonly SteamDataPage _steamDataPage;
        private readonly UsbDevicePage _usbDevicesPage;

        public MainWindow(
            ILogger<MainWindow> logger,
            GreetingPage greetingPage,
            SteamDataPage steamDataPage,
            MouseLoggerPage mouseLoggerPage,
            LastActivityPage lastActivityPage,
            UsbDevicePage usbDevicesPage,
            SearchFilePage searchFilePage,
            OtherPage otherPage
        )
        {
            InitializeComponent();
            _logger = logger;
            _steamDataPage = steamDataPage;
            _mouseLoggerPage = mouseLoggerPage;
            _lastActivityPage = lastActivityPage;
            _usbDevicesPage = usbDevicesPage;
            _searchFilePage = searchFilePage;
            _otherPage = otherPage;
            WindowsPageManager(greetingPage);
            _logger.LogInformation("MainWindow has been loaded successfully");
        }

        private void window_loaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);

        private void WindowsPageManager(Page newPage)
        {
            _logger.LogInformation("Has been opened page: {@NewPage}", newPage);
            PagesFrame.Content = newPage;
        }

        private void button_show_account_info_page_MouseDown(object sender, MouseButtonEventArgs e) =>
            WindowsPageManager(_steamDataPage);

        private void button_show_usb_device_page_MouseDown(object sender, MouseButtonEventArgs e) =>
            WindowsPageManager(_usbDevicesPage);

        private void button_show_last_activity_page_MouseDown(object sender, MouseButtonEventArgs e) =>
            WindowsPageManager(_lastActivityPage);

        private void button_show_mouse_check_MouseDown(object sender, MouseButtonEventArgs e) =>
            WindowsPageManager(_mouseLoggerPage);

        private void Button_show_search_file_OnMouseDown(object sender, MouseButtonEventArgs e) =>
            WindowsPageManager(_searchFilePage);

        private void button_show_other_page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            WindowsPageManager(_otherPage);

        private void grid_custom_title_bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void label_close_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            Environment.Exit(Environment.ExitCode);

        private void label_turn_off_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}