using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Xml.Linq;

namespace RCC
{
    public partial class MainWindow : Window
    {
        public class usb_device_info
        {
            public string device_name { get; set; }
            public string description { get; set; }
            public string device_type { get; set; }
            public bool is_connect { get; set; }
            public string time_last_used { get; set; }
            public string creating_time { get; set; }

            public usb_device_info(string device_name, string description, string device_type, bool is_connect, string time_last_used, string creating_time)
            {
                this.device_name = device_name;
                this.description = description;
                this.device_type = device_type;
                this.is_connect = is_connect;
                this.time_last_used = time_last_used;
                this.creating_time = creating_time;
            }
        }

        public class last_activity_info
        {
            public string action_time { get; set; }
            public string description { get; set; }
            public string filename { get; set; }
            public string full_path { get; set; }

            public last_activity_info(string action_time, string description, string filename, string full_path)
            {
                this.action_time = action_time;
                this.description = description;
                this.filename = filename;
                this.full_path = full_path;
            }
        }
     
        private void window_loaded(object sender, RoutedEventArgs e) => GlassEffect.EnableBlur(this);

        private readonly BackgroundWorker background_worker_find_steam_account = new BackgroundWorker();
        private readonly BackgroundWorker background_worker_find_usb_device = new BackgroundWorker();
        private readonly BackgroundWorker background_worker_find_last_activity = new BackgroundWorker();

        void background_worker_find_steam_account_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Steam.LocalInfo.SteamData steam = e.UserState as Steam.LocalInfo.SteamData;
            list_other_accounts.Items.Add(new Steam.LocalInfo.SteamData(steam.username, steam.steam_id, steam.account_level, steam.avatar_url, steam.is_hide_account));
        }
        void background_worker_find_usb_device_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            usb_device_info usb_data = e.UserState as usb_device_info;
            list_all_usb_device.Items.Add(new usb_device_info(usb_data.device_name, usb_data.description, usb_data.device_type, usb_data.is_connect, usb_data.time_last_used, usb_data.creating_time));
        }
        void background_worker_find_last_activity_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            last_activity_info last_activity_info = e.UserState as last_activity_info;
            list_all_last_activity.Items.Add(new last_activity_info(last_activity_info.action_time, last_activity_info.description, last_activity_info.filename, last_activity_info.full_path));
        }
        void thread_to_remove_resources(string[] path_array)
        {
            Thread delete_thread = new Thread(remove_file_list =>
            {
                foreach (string file in (remove_file_list as string[]))
                {
                    Thread.Sleep(500);
                    File.Delete(file);
                }

            });
            delete_thread.Start(path_array);
        }
        void background_worker_find_usb_device_DoWork(object sender, DoWorkEventArgs e)
        {
            string path_to_local_application = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.ToString());
            string local_path_to_file = $"{path_to_local_application}\\USBDeview.exe";
            string path_to_save_usb_list = $"{path_to_local_application}\\usb_info.xml";
            string argument_to_startup = $"/sxml {path_to_save_usb_list}";
            File.WriteAllBytes(local_path_to_file, Properties.Resources.USBDeview);
            Process.Start(local_path_to_file, argument_to_startup).WaitForExit();

            thread_to_remove_resources(new string[] { local_path_to_file, path_to_save_usb_list });

            var load_xml_document = XDocument.Load(path_to_save_usb_list).Descendants("item");

            int i = 0;
            foreach (XElement element in load_xml_document)
            {
                string device_name = element.Element("device_name").Value;
                string description = element.Element("description").Value;
                string device_type = element.Element("device_type").Value;
                bool is_connect = element.Element("connected").Value == "Yes";
                string time_last_used = element.Element("last_plug_unplug_date").Value;
                string creating_time = element.Element("created_date").Value;
                usb_device_info device_info = new usb_device_info(device_name, description, device_type, is_connect, time_last_used, creating_time);
                background_worker_find_usb_device.ReportProgress(i, device_info);
                i++;
            }
        }
        void background_worker_find_last_activity_DoWork(object sender, DoWorkEventArgs e)
        {
            string path_to_local_application = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.ToString());
            string local_path_to_file = $"{path_to_local_application}\\LastActivityView.exe";
            string path_to_save_usb_list = $"{path_to_local_application}\\last_activity_view.xml";
            string argument_to_startup = $"/sxml {path_to_save_usb_list}";
            File.WriteAllBytes(local_path_to_file, Properties.Resources.LastActivityView);
            Process.Start(local_path_to_file, argument_to_startup).WaitForExit();

            thread_to_remove_resources(new string[] { local_path_to_file, path_to_save_usb_list });

            var load_xml_document = XDocument.Load(path_to_save_usb_list).Descendants("item");

            int i = 0;
            foreach (XElement element in load_xml_document)
            {
                string action_time = element.Element("action_time").Value;
                string description = element.Element("description").Value;
                string filename = element.Element("filename").Value;
                string full_path = element.Element("full_path").Value;

                if (!File.Exists(full_path))
                    full_path = "File has been removed";

                if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(full_path))
                    continue;

                last_activity_info info = new last_activity_info(action_time, description, filename, full_path);
                background_worker_find_last_activity.ReportProgress(i, info);
                i++;
            }
        }
        void background_worker_find_steam_account_DoWork(object sender, DoWorkEventArgs e)
        {
            string steam_path_to_login_user = Steam.LocalInfo.get_path_to_login();
            if (!File.Exists(steam_path_to_login_user))
                return;
            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = Steam.LocalInfo.get_all_steam_id(file_data);
            for (int i = 0; i < get_steam_id_data.Count; i++)
                background_worker_find_steam_account.ReportProgress(i,Steam.LocalInfo.parse_from_steam(long.Parse(get_steam_id_data[i])));
        }
        public MainWindow()
        {
            InitializeComponent();
            background_worker_find_steam_account.DoWork += new DoWorkEventHandler(background_worker_find_steam_account_DoWork);
            background_worker_find_steam_account.ProgressChanged += new ProgressChangedEventHandler(background_worker_find_steam_account_ProgressChanged);
            background_worker_find_steam_account.WorkerReportsProgress = true;
            background_worker_find_steam_account.RunWorkerAsync();

            background_worker_find_usb_device.DoWork += new DoWorkEventHandler(background_worker_find_usb_device_DoWork);
            background_worker_find_usb_device.ProgressChanged += new ProgressChangedEventHandler(background_worker_find_usb_device_ProgressChanged);
            background_worker_find_usb_device.WorkerReportsProgress = true;
            background_worker_find_usb_device.RunWorkerAsync();

            background_worker_find_last_activity.DoWork += new DoWorkEventHandler(background_worker_find_last_activity_DoWork);
            background_worker_find_last_activity.ProgressChanged += new ProgressChangedEventHandler(background_worker_find_last_activity_ProgressChanged);
            background_worker_find_last_activity.WorkerReportsProgress = true;
            background_worker_find_last_activity.RunWorkerAsync();

            label_full_path_to_steam.Content = Steam.LocalInfo.get_steam_location();
            Steam.LocalInfo.SteamData last_account_info = Steam.LocalInfo.get_last_account_info();
            label_steam_account_steam_id.Content = last_account_info.get_steam_id;
            label_steam_account_username.Content = last_account_info.get_username;
            label_cpu_type.Content = GetSysthemInfo.get_cpu_name;
            label_gpu_type.Content = GetSysthemInfo.get_gpu_name;
            label_screen_size.Content = GetSysthemInfo.get_screen_size;
            label_windows_type.Content = GetSysthemInfo.get_os_type;
            label_memory_size.Content = GetSysthemInfo.get_ram_size;
            label_start_up_time.Content = GetSysthemInfo.get_system_start_up;
            label_user_ip.Content = GetSysthemInfo.get_user_external_ip();
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = last_account_info.get_account_avatar;
            rectangle_local_profile_image.Fill = myBrush;
        }

        private void window_page_manager(Grid show_grid)
        {
            grid_last_activity.Visibility = Visibility.Hidden;
            grid_greeting.Visibility = Visibility.Hidden;
            grid_accont_info.Visibility = Visibility.Hidden;
            grid_usb_device.Visibility = Visibility.Hidden;
            show_grid.Visibility = Visibility.Visible;
        }

        private void windows_title_bar(object sender, MouseButtonEventArgs e) => DragMove();
        private void button_open_steam_path_Click(object sender, RoutedEventArgs e) => Process.Start(Steam.LocalInfo.get_steam_location());
        private void button_show_account_info_page_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_accont_info);
        private void button_show_usb_device_page_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_usb_device);
        private void button_show_last_activity_page_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_last_activity);
    }
}
