using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using MessageBox = RCC.windows.MessageBox;

namespace RCC.Pages
{
    public partial class SearchFilePage : Page
    {
        private bool _isStartSearch = false;
        private readonly BackgroundWorker _backgroundWorkerFindAllFiles = new BackgroundWorker();

        void BackgroundWorkerFindAllFilesProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            file_information information = e.UserState as file_information;
            
            if (information == null) 
                return;
            
            ListAllFileSearch.Items.Add(new file_information(information.file_name, information.create_date,
                information.directory, information.size));
        }
        
        void BackgroundWorkerFindAllFilesDoWork(object sender, DoWorkEventArgs e)
        {
            while (!_isStartSearch)
            {
                Thread.Sleep(100);
            }
            
            new MessageBox().Show("Сканирование системы началось");
            
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
                    List<SearcherFiles> searchList = new List<SearcherFiles>();
                    // create new thread list
                    List<Thread> threadList = new List<Thread>();

                    // for each folder in the root directory. Creating a class search and thread
                    getSubdirectories.ForEach(sub_directory =>
                    {
                        SearcherFiles s1 = new SearcherFiles(sub_directory, "*.*", DateTime.MinValue, DateTime.MaxValue, 0, Int32.MaxValue);
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
                        _backgroundWorkerFindAllFiles.ReportProgress(counter, file);
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
                SearcherFiles ls = new SearcherFiles(directory, "*.*", DateTime.MinValue, DateTime.MaxValue, 0, Int32.MaxValue);
                List<file_information> localFile = ls.search_files(directory); // Search for files only in the root directory
                localFile.ForEach(file =>
                {
                    _backgroundWorkerFindAllFiles.ReportProgress(counter, file);
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
        public SearchFilePage()
        {
            InitializeComponent();
            _backgroundWorkerFindAllFiles.DoWork += BackgroundWorkerFindAllFilesDoWork;
            _backgroundWorkerFindAllFiles.ProgressChanged += BackgroundWorkerFindAllFilesProgressChanged;
            _backgroundWorkerFindAllFiles.WorkerReportsProgress = true;
            _backgroundWorkerFindAllFiles.RunWorkerAsync();
            ListAllFileSearch.Items.Filter = CustomFilter;
        }

        private void ButtonRunCheck_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.GridStartSearching.Visibility = Visibility.Hidden;
            this.GridSearching.Visibility = Visibility.Visible;
            _isStartSearch = true;
        }
        private bool CustomFilter(object obj)   
        {  
            if (string.IsNullOrEmpty(SearchFileTextBoxFilter.Text))
                return true;
            return ((file_information)obj).file_name.IndexOf(SearchFileTextBoxFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        } 
        private void SearchFileTextBoxFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ListAllFileSearch.Items.Filter = CustomFilter;
            ListAllFileSearch.Items.Refresh();
        }
    }
}