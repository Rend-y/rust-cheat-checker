using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RCC
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 1,
        ACCENT_ENABLE_GRADIENT = 0,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }

    public class SteamAccount
    {
        public string user_name;
        public long steam_id;
        public string get_user_name => $"Username : {this.user_name}";
        public string get_steam_id => $"Steam Id : {this.steam_id}";
    }
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
        private void window_loaded(object sender, RoutedEventArgs e) => EnableBlur();

        public MainWindow()
        {
            InitializeComponent();
            label_full_path_to_steam.Content = Steam.LocalInfo.get_steam_location();
            var last_account_info = Steam.LocalInfo.get_last_account_info();
            label_steam_account_steam_id.Content = last_account_info.get_steam_id;
            label_steam_account_username.Content = last_account_info.get_username;
            label_cpu_type.Content = GetSysthemInfo.get_cpu_name;
            label_gpu_type.Content = GetSysthemInfo.get_gpu_name;
            label_screen_size.Content = GetSysthemInfo.get_screen_size;
            label_windows_type.Content = GetSysthemInfo.get_os_type;
            label_memory_size.Content = GetSysthemInfo.get_ram_size;
            label_start_up_time.Content = GetSysthemInfo.get_system_start_up;
            label_user_ip.Content = GetSysthemInfo.get_user_external_ip();
            list_other_accounts.ItemsSource = Steam.LocalInfo.get_steam_all_steam_account();
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = last_account_info.avatar.Source;
            rectangle_local_profile_image.Fill = myBrush;
        }

        private void windows_title_bar(object sender, MouseButtonEventArgs e) => DragMove();

        private void button_open_steam_path_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Steam.LocalInfo.get_steam_location());
        }
    }
}
