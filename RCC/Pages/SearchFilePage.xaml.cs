using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using RCC.Modules.FileSearcher;

namespace RCC.Pages
{
    public partial class SearchFilePage : Page
    {
        private readonly BackgroundWorker _backgroundWorkerFindAllFiles = new();
        private bool _isStartSearch = false;

        public SearchFilePage()
        {
            InitializeComponent();
            _backgroundWorkerFindAllFiles.DoWork += BackgroundWorkerFindAllFilesDoWork;
            _backgroundWorkerFindAllFiles.ProgressChanged += BackgroundWorkerFindAllFilesProgressChanged;
            _backgroundWorkerFindAllFiles.WorkerReportsProgress = true;
            _backgroundWorkerFindAllFiles.RunWorkerAsync();
            ListAllFileSearch.Items.Filter = CustomFilter;
        }

        void BackgroundWorkerFindAllFilesProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var information = e.UserState as FileInformation;
            if (information == null)
                return;
            ListAllFileSearch.Items.Add(new FileInformation(
                    information.FileName,
                    information.CreateDate,
                    information.Directory,
                    information.Size
                )
            );
        }

        void BackgroundWorkerFindAllFilesDoWork(object sender, DoWorkEventArgs e)
        {
            while (!_isStartSearch)
            {
                Thread.Sleep(100);
            }

            // TODO: is not STA thread 
            // new MessageBox("Сканирование системы началось").Show();

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
                    var searchList = new List<FileSearcher>();
                    // create new thread list
                    List<Thread> threadList = new List<Thread>();

                    // for each folder in the root directory. Creating a class search and thread
                    getSubdirectories.ForEach(subDirectory =>
                    {
                        var s1 = new FileSearcher(subDirectory, DateTime.MinValue, DateTime.MaxValue,
                            "*.*", 0, int.MaxValue);
                        searchList.Add(s1);
                        var t1 = new Thread(s1.Run);
                        t1.SetApartmentState(ApartmentState.STA);
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

                    searchList.ForEach(search => search.FindFileList.ForEach(file =>
                    {
                        _backgroundWorkerFindAllFiles.ReportProgress(counter, file);
                        XElement rootClass = new XElement("file");
                        var filename = new XElement("filename", file.FileName);
                        var createDate = new XElement("create-data", file.CreateDate);
                        var directories = new XElement("directory", file.Directory);
                        var fileSize = new XElement("file-size", file.Size);
                        rootClass.Add(filename);
                        rootClass.Add(createDate);
                        rootClass.Add(directories);
                        rootClass.Add(fileSize);
                        rootList.Add(rootClass);
                        counter++;
                    }));
                }

                // creating a new searches for find all files in the root directory
                var ls = new FileSearcher(directory, DateTime.MinValue, DateTime.MaxValue, "*.*", 0,
                    Int32.MaxValue);
                var
                    localFile = ls.SearchFile(directory); // Search for files only in the root directory
                localFile.ForEach(file =>
                {
                    _backgroundWorkerFindAllFiles.ReportProgress(counter, file);
                    XElement rootClass = new XElement("file");
                    var filename = new XElement("filename", file.FileName);
                    var createDate = new XElement("create-data", file.CreateDate);
                    var directories = new XElement("directory", file.Directory);
                    var fileSize = new XElement("file-size", file.Size);
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
            return ((FileInformation)obj).FileName.IndexOf(SearchFileTextBoxFilter.Text,
                StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SearchFileTextBoxFilter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ListAllFileSearch.Items.Filter = CustomFilter;
            ListAllFileSearch.Items.Refresh();
        }
    }
}