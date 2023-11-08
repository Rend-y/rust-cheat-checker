using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RCC.Modules.FileSearcher;

namespace RCC.Pages
{
    public partial class SearchFilePage : Page
    {
        private readonly BackgroundWorker _backgroundWorkerFindAllFiles = new();
        private readonly IFileSearcher<FileInformation> _fileSearcher;
        private bool _isStartSearch = false;

        public SearchFilePage(IFileSearcher<FileInformation> fileSearcher)
        {
            InitializeComponent();
            _fileSearcher = fileSearcher;
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

        private void Start()
        {
            Debug.WriteLine("start searching file");
            while (!_isStartSearch) Thread.Sleep(100);

            // TODO: is not STA thread 
            // new MessageBox("Сканирование системы началось").Show();

            List<DriveInfo> allDriver = DriveInfo.GetDrives().ToList();
            List<DirectoryInfo> listAllDisc = new List<DirectoryInfo>();
            listAllDisc.Add(new DirectoryInfo("C:\\"));

            listAllDisc.ForEach(directory =>
            {
                _fileSearcher.Run(directory, DateTime.MinValue, DateTime.MaxValue,
                    "*.*", 0, int.MaxValue);
                var i = 0;
                _fileSearcher.FindFileList.ForEach(file =>
                {
                    _backgroundWorkerFindAllFiles.ReportProgress(i, file);
                    i++;
                });
            });
        }

        private void BackgroundWorkerFindAllFilesDoWork(object sender, DoWorkEventArgs e)
        {
            Start();
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