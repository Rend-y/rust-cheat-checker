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
        public class LastActivityInfo
        {
            public string action_time { get; set; }
            public string description { get; set; }
            public string filename { get; set; }
            public string full_path { get; set; }

            public LastActivityInfo(string action_time, string description, string filename, string full_path)
            {
                this.action_time = action_time;
                this.description = description;
                this.filename = filename;
                this.full_path = full_path;
            }
        }
        private void window_loaded(object sender, RoutedEventArgs e) => glass_effect.enable_blur(this);
        private readonly BackgroundWorker background_worker_find_all_files = new BackgroundWorker();
        private readonly SteamDataPage steamDataPage = new SteamDataPage();
        private readonly MouseLoggerPage mouseLoggerPage = new MouseLoggerPage();
        private readonly LastActivityPage lastActivityPage = new LastActivityPage();
        private readonly UsbDevicePage usbDevicesPage = new UsbDevicePage();
        void background_worker_find_all_files_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            file_information information = e.UserState as file_information;
            
            if (information == null) 
                return;
            
            list_all_file_search.Items.Add(new file_information(information.file_name, information.create_date,
                information.directory, information.size));
        }
        IEnumerable<XElement> get_xml_document_from_web_process(string path_to_exe, string url, string path_to_save_xml)
        {
            new WebClient().DownloadFile(url, path_to_exe);
            Process.Start(path_to_exe, $"/sxml {path_to_save_xml}")?.WaitForExit();

            Thread deleteThread = new Thread(remove_file_list =>
            {
                if (remove_file_list is string[] fileList)
                {
                    fileList.ToList().ForEach(file =>
                    {
                        Thread.Sleep(500);
                        File.Delete(file);
                    });
                }
            });
            deleteThread.Start(new[] { path_to_exe, path_to_save_xml });

            return XDocument.Load(path_to_save_xml).Descendants("item");
        }

        static string PathToLocalApplication => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        void background_worker_find_all_files_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            
            XDocument document = new XDocument();
            XElement rootList = new XElement("items");
            
            List<DriveInfo> allDriver = DriveInfo.GetDrives().ToList();
            List<DirectoryInfo> listAllDisc = new List<DirectoryInfo>();
            allDriver.ForEach(driver =>
            {
                if (driver.DriveType == DriveType.Fixed)
                    listAllDisc.Add(new DirectoryInfo(driver.Name));
            });
            
            listAllDisc.Add(new DirectoryInfo("C:\\"));

            listAllDisc.ForEach(directory =>
            {
                // if at the directory have sub directories. Then run the following code
                List<DirectoryInfo> getSubdirectories = directory.GetDirectories().ToList();
                if (getSubdirectories.Count != 0)
                {
                    // create new search list
                    List<searcher_files> searchList = new List<searcher_files>();
                    // create new thread list
                    List<Thread> threadList = new List<Thread>();

                    // for each folder in the root directory. Creating a class search and thread
                    getSubdirectories.ForEach(sub_directory =>
                    {
                        searcher_files s1 = new searcher_files(sub_directory, "*.*", DateTime.MinValue, DateTime.MaxValue, 0, Int32.MaxValue);
                        searchList.Add(s1);
                        Thread t1 = new Thread(s1.start_search);
                        threadList.Add(t1);
                    });

                    threadList.ForEach(thread => thread.Start()); // runin all threads

                    // waiting for all threads to finish
                    while (true)
                    {
                        int endCount = 0;
                        threadList.ForEach(thread =>
                        {
                            if (thread.ThreadState == ThreadState.Running)
                                endCount++;
                        });
                        if (endCount == 0) break;
                    }
                    searchList.ForEach(search => search.find_files.ForEach(file =>
                    {
                        background_worker_find_all_files.ReportProgress(counter, file);
                        XElement rootClass = new XElement("file");
                        XElement filename = new XElement("filename", file.file_name);
                        XElement createDate = new XElement("create-data", file.create_date);
                        XElement directories = new XElement("directory", file.directory);
                        XElement fileSize = new XElement("file-size", file.size);
                        rootClass.Add(filename);
                        rootClass.Add(createDate);
                        rootClass.Add(directories);
                        rootClass.Add(fileSize);
                        rootList.Add(rootClass);
                        counter++;
                    }));
                }

                // creating a new searches for find all files in the root directory
                searcher_files ls = new searcher_files(directory, "*.*", DateTime.MinValue, DateTime.MaxValue, 0, Int32.MaxValue);
                List<file_information> localFile = ls.search_files(directory); // Search for files only in the root directory
                localFile.ForEach(file =>
                {
                    background_worker_find_all_files.ReportProgress(counter, file);
                    XElement rootClass = new XElement("file");
                    XElement filename = new XElement("filename", file.file_name);
                    XElement createDate = new XElement("create-data", file.create_date);
                    XElement directories = new XElement("directory", file.directory);
                    XElement fileSize = new XElement("file-size", file.size);
                    rootClass.Add(filename);
                    rootClass.Add(createDate);
                    rootClass.Add(directories);
                    rootClass.Add(fileSize);
                    rootList.Add(rootClass);
                    counter++;
                });
            });
            
            document.Add(rootList);
            document.Save("file_list.xml");
        }
        public main_window()
        {
            InitializeComponent();

            WindowsPageManager(new GreetingPage());

            // background_worker_find_all_files.DoWork += background_worker_find_all_files_DoWork;
            // background_worker_find_all_files.ProgressChanged += background_worker_find_all_files_ProgressChanged;
            // background_worker_find_all_files.WorkerReportsProgress = true;
            // background_worker_find_all_files.RunWorkerAsync();
            
            dangerous_apps.start_scan().ForEach(item => list_all_dangerous_apps.Items.Add(item));
        }
        private void WindowsPageManager(Page newPage) => PagesFrame.Content = newPage; 
        private void button_show_account_info_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(steamDataPage);
        private void button_show_usb_device_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(usbDevicesPage);
        private void button_show_last_activity_page_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(lastActivityPage);
        private void button_show_mouse_check_MouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(mouseLoggerPage);
        private void grid_custom_title_bar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void label_close_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Environment.Exit(Environment.ExitCode);
        private void label_turn_off_application_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.WindowState = WindowState.Minimized;
        private void Button_show_search_file_OnMouseDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(new SteamDataPage());
        private void button_show_other_page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowsPageManager(new SteamDataPage());
    }
}
