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
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using RCC.Steam;
using Path = System.IO.Path;
using ThreadState = System.Threading.ThreadState;

namespace RCC
{
    public partial class main_window : Window
    {
        public class mouse_activity {
            public string key { get; set; }
            public mouse_activity(string key)
            {
                this.key = key;
            }
        }
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
        private void window_loaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);
        private readonly BackgroundWorker background_worker_find_steam_account = new BackgroundWorker();
        private readonly BackgroundWorker background_worker_find_usb_device = new BackgroundWorker();
        private readonly BackgroundWorker background_worker_find_last_activity = new BackgroundWorker();
        private readonly BackgroundWorker background_worker_find_all_files = new BackgroundWorker();
        
        void background_worker_find_all_files_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            file_information information = e.UserState as file_information;
            
            if (information == null) 
                return;
            
            list_all_file_search.Items.Add(new file_information(information.file_name, information.create_date,
                information.directory, information.size));
        }      
        void background_worker_find_steam_account_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            steam_data steam = e.UserState as steam_data;
            
            if (steam == null)
                return;
            
            list_other_accounts.Items.Add(new steam_data(steam.username, steam.steam_id, steam.account_level, steam.avatar_url, steam.is_hide_account));
        }
        void background_worker_find_usb_device_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            usb_device_info usb_data = e.UserState as usb_device_info;
            
            if (usb_data == null)
                return;

            list_all_usb_device.Items.Add(new usb_device_info(usb_data.device_name, usb_data.description, usb_data.device_type, usb_data.is_connect, usb_data.time_last_used, usb_data.creating_time));
        }
        void background_worker_find_last_activity_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            last_activity_info last_activity_info = e.UserState as last_activity_info;
            
            if (last_activity_info == null)
                return;
            
            list_all_last_activity.Items.Add(new last_activity_info(last_activity_info.action_time, last_activity_info.description, last_activity_info.filename, last_activity_info.full_path));
        }
        IEnumerable<XElement> get_xml_document_from_web_process(string path_to_exe, string url, string path_to_save_xml)
        {
            new WebClient().DownloadFile(url, path_to_exe);
            Process.Start(path_to_exe, $"/sxml {path_to_save_xml}")?.WaitForExit();

            Thread delete_thread = new Thread(remove_file_list =>
            {
                if (remove_file_list is string[] file_list)
                {
                    file_list.ToList().ForEach(file =>
                    {
                        Thread.Sleep(500);
                        File.Delete(file);
                    });
                }
            });
            delete_thread.Start(new[] { path_to_exe, path_to_save_xml });

            return XDocument.Load(path_to_save_xml).Descendants("item");
        }
        string path_to_local_application => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        void background_worker_find_all_files_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            
            XDocument document = new XDocument();
            XElement root_list = new XElement("items");
            
            List<DriveInfo> all_driver = DriveInfo.GetDrives().ToList();
            List<DirectoryInfo> list_all_disc = new List<DirectoryInfo>();
            all_driver.ForEach(driver =>
            {
                if (driver.DriveType == DriveType.Fixed)
                    list_all_disc.Add(new DirectoryInfo(driver.Name));
            });
            
            list_all_disc.Add(new DirectoryInfo("C:\\"));

            list_all_disc.ForEach(directory =>
            {
                // if at the directory have sub directories. Then run the following code
                List<DirectoryInfo> get_subdirectories = directory.GetDirectories().ToList();
                if (get_subdirectories.Count != 0)
                {
                    // create new search list
                    List<searcher_files> search_list = new List<searcher_files>();
                    // create new thread list
                    List<Thread> thread_list = new List<Thread>();

                    // for each folder in the root directory. Creating a class search and thread
                    get_subdirectories.ForEach(sub_directory =>
                    {
                        searcher_files s1 = new searcher_files(sub_directory, "*.*", DateTime.MinValue, DateTime.MaxValue, 0, Int32.MaxValue);
                        search_list.Add(s1);
                        Thread t1 = new Thread(s1.start_search);
                        thread_list.Add(t1);
                    });

                    thread_list.ForEach(thread => thread.Start()); // runin all threads

                    // waiting for all threads to finish
                    while (true)
                    {
                        int end_count = 0;
                        thread_list.ForEach(thread =>
                        {
                            if (thread.ThreadState == ThreadState.Running)
                                end_count++;
                        });
                        if (end_count == 0) break;
                    }
                    search_list.ForEach(search => search.find_files.ForEach(file =>
                    {
                        background_worker_find_all_files.ReportProgress(counter, file);
                        XElement root_class = new XElement("file");
                        XElement filename = new XElement("filename", file.file_name);
                        XElement create_date = new XElement("create-data", file.create_date);
                        XElement directories = new XElement("directory", file.directory);
                        XElement file_size = new XElement("file-size", file.size);
                        root_class.Add(filename);
                        root_class.Add(create_date);
                        root_class.Add(directories);
                        root_class.Add(file_size);
                        root_list.Add(root_class);
                        counter++;
                    }));
                }

                // creating a new searches for find all files in the root directory
                searcher_files ls = new searcher_files(directory, "*.*", DateTime.MinValue, DateTime.MaxValue, 0, Int32.MaxValue);
                List<file_information> local_file = ls.search_files(directory); // Search for files only in the root directory
                local_file.ForEach(file =>
                {
                    background_worker_find_all_files.ReportProgress(counter, file);
                    XElement root_class = new XElement("file");
                    XElement filename = new XElement("filename", file.file_name);
                    XElement create_date = new XElement("create-data", file.create_date);
                    XElement directories = new XElement("directory", file.directory);
                    XElement file_size = new XElement("file-size", file.size);
                    root_class.Add(filename);
                    root_class.Add(create_date);
                    root_class.Add(directories);
                    root_class.Add(file_size);
                    root_list.Add(root_class);
                    counter++;
                });
            });
            
            document.Add(root_list);
            document.Save("file_list.xml");
        }
        void background_worker_find_usb_device_DoWork(object sender, DoWorkEventArgs e)
        {
            string local_path_to_file = $"{path_to_local_application}\\USBDeview.exe";
            string path_to_save_usb_list = $"{path_to_local_application}\\usb_info.xml";
            var load_xml_document = get_xml_document_from_web_process(local_path_to_file, "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/USBDeview.exe?raw=true", path_to_save_usb_list);

            int i = 0;
            foreach (XElement element in load_xml_document)
            {
                string device_name = element.Element("device_name")?.Value;
                string description = element.Element("description")?.Value;
                string device_type = element.Element("device_type")?.Value;
                bool is_connect = element.Element("connected")?.Value == "Yes";   
                string time_last_used = element.Element("last_plug_unplug_date")?.Value;
                string creating_time = element.Element("created_date")?.Value;
                usb_device_info device_info = new usb_device_info(device_name, description, device_type, is_connect, time_last_used, creating_time);
                background_worker_find_usb_device.ReportProgress(i, device_info);
                i++;
            }
        }
        void background_worker_find_last_activity_DoWork(object sender, DoWorkEventArgs e)
        {
            string local_path_to_file = $"{path_to_local_application}\\LastActivityView.exe";
            string path_to_save_usb_list = $"{path_to_local_application}\\last_activity_view.xml";
            var load_xml_document = get_xml_document_from_web_process(local_path_to_file, "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/LastActivityView.exe?raw=true", path_to_save_usb_list);

            int i = 0;
            foreach (XElement element in load_xml_document)
            {
                string action_time = element.Element("action_time")?.Value;
                string description = element.Element("description")?.Value;
                string filename = element.Element("filename")?.Value;
                string full_path = element.Element("full_path")?.Value;

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
            string steam_path_to_login_user = local_info.get_path_to_login_user();
            if (!File.Exists(steam_path_to_login_user))
                return;
            string file_data = File.ReadAllText(steam_path_to_login_user);
            List<string> get_steam_id_data = local_info.get_all_steam_id(file_data);

            int i = 0;
            get_steam_id_data.ForEach(steam_id =>
            {
                background_worker_find_steam_account.ReportProgress(i, local_info.parse_from_steam(long.Parse(steam_id)));
                i++;
            });
        }
        public main_window()
        {
            InitializeComponent();

            window_page_manager(grid_greeting);

            background_worker_find_steam_account.DoWork += background_worker_find_steam_account_DoWork;
            background_worker_find_steam_account.ProgressChanged += background_worker_find_steam_account_ProgressChanged;
            background_worker_find_steam_account.WorkerReportsProgress = true;
            background_worker_find_steam_account.RunWorkerAsync();

            background_worker_find_usb_device.DoWork += background_worker_find_usb_device_DoWork;
            background_worker_find_usb_device.ProgressChanged += background_worker_find_usb_device_ProgressChanged;
            background_worker_find_usb_device.WorkerReportsProgress = true;
            background_worker_find_usb_device.RunWorkerAsync();

            background_worker_find_last_activity.DoWork += background_worker_find_last_activity_DoWork;
            background_worker_find_last_activity.ProgressChanged += background_worker_find_last_activity_ProgressChanged;
            background_worker_find_last_activity.WorkerReportsProgress = true;
            background_worker_find_last_activity.RunWorkerAsync();

            background_worker_find_all_files.DoWork += background_worker_find_all_files_DoWork;
            background_worker_find_all_files.ProgressChanged += background_worker_find_all_files_ProgressChanged;
            background_worker_find_all_files.WorkerReportsProgress = true;
            background_worker_find_all_files.RunWorkerAsync();
            
            label_full_path_to_steam.Content = local_info.get_steam_location();
            steam_data last_account_info = local_info.get_last_account_info();
            dangerous_apps.start_scan().ForEach(item => list_all_dangerous_apps.Items.Add(item));
            label_steam_account_steam_id.Content = last_account_info.get_steam_id;
            label_steam_account_username.Content = last_account_info.get_username;
            label_cpu_type.Content = get_system_info.get_cpu_name;
            label_gpu_type.Content = get_system_info.get_gpu_name;
            label_screen_size.Content = get_system_info.get_screen_size;
            label_windows_type.Content = get_system_info.get_os_type;
            label_memory_size.Content = get_system_info.get_ram_size;
            label_start_up_time.Content = get_system_info.get_system_start_up;
            label_user_ip.Content = get_system_info.get_user_external_ip();
            ImageBrush my_brush = new ImageBrush
            {
                ImageSource = last_account_info.get_account_avatar
            };
            rectangle_local_profile_image.Fill = my_brush;
        }
        private void window_page_manager(Grid show_grid)
        {
            grid_logger_mouse_move.Visibility = Visibility.Hidden;
            grid_last_activity.Visibility = Visibility.Hidden;
            grid_greeting.Visibility = Visibility.Hidden;
            grid_accont_info.Visibility = Visibility.Hidden;
            grid_usb_device.Visibility = Visibility.Hidden;
            grid_search_file.Visibility = Visibility.Hidden;
            grid_other.Visibility = Visibility.Hidden;
            show_grid.Visibility = Visibility.Visible;
        }
        private void button_open_steam_path_Click(object sender, RoutedEventArgs e) => Process.Start(local_info.get_steam_location());
        private void button_show_account_info_page_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_accont_info);
        private void button_show_usb_device_page_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_usb_device);
        private void button_show_last_activity_page_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_last_activity);
        private void button_show_mouse_check_MouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_logger_mouse_move);
        private void canvas_mouse_drawing_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line
                {
                    Stroke = SystemColors.WindowFrameBrush,
                    X1 = e.GetPosition(canvas_mouse_drawing).X - 1,
                    Y1 = e.GetPosition(canvas_mouse_drawing).Y - 1,
                    X2 = e.GetPosition(canvas_mouse_drawing).X,
                    Y2 = e.GetPosition(canvas_mouse_drawing).Y
                };

                canvas_mouse_drawing.Children.Add(line);
            }
        }
        private void canvas_mouse_drawing_MouseUp(object sender, MouseEventArgs e) => canvas_mouse_drawing.Children.Clear();
            private void canvas_mouse_drawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string conversion_event_to_string;
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    conversion_event_to_string = "Пользователь нажал правую кнопку";
                    break;
                case MouseButton.Left:
                    conversion_event_to_string = "Пользователь нажал левую кнопку";
                    break;
                case MouseButton.Middle:
                    conversion_event_to_string = "Пользователь нажал центральную кнопку";
                    break;
                case MouseButton.XButton1:
                    conversion_event_to_string = "Пользователь нажал боковую (1) кнопку";
                    break;
                case MouseButton.XButton2:
                    conversion_event_to_string = "Пользователь нажал боковую (2) кнопку";
                    break;
                default:
                    conversion_event_to_string = "Пользователь нажал неизвестную кнопку";
                    break;
            }
            list_all_mouse_event.Items.Insert(0, new mouse_activity(conversion_event_to_string));
            if (list_all_mouse_event.Items.Count > 10) list_all_mouse_event.Items.RemoveAt(list_all_mouse_event.Items.Count - 1);
        }
        private void grid_custom_title_bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (grid_logger_mouse_move.Visibility == Visibility.Visible)
                return;
            DragMove();
        }
        private void list_other_accounts_SelectionChanged(object sender, SelectionChangedEventArgs e) => Process.Start($"https://steamcommunity.com/profiles/{((steam_data)list_other_accounts.SelectedItem).steam_id}");
        private void label_close_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Environment.Exit(Environment.ExitCode);
        private void label_turn_off_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;
        private void Button_show_search_file_OnMouseDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_search_file);
        private void button_to_redirect_on_git_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
            Process.Start("https://github.com/Midoruya/rust-cheat-checker/releases/latest/");
        private void button_show_other_page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => window_page_manager(grid_other);
    }
}
