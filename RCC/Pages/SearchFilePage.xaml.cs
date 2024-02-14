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
using Microsoft.Extensions.Logging;
using RCC.Modules.FileSearcher;

namespace RCC.Pages
{
    public partial class SearchFilePage : APage
    {
        private readonly IFileSearcher<FileInformation> _fileSearcher;
        private bool _isStartSearch = false;

        public SearchFilePage(IFileSearcher<FileInformation> fileSearcher, ILogger<SearchFilePage> logger) : base(logger)
        {
            InitializeComponent();
            RunBackgroundWorker();
            _fileSearcher = fileSearcher;
            ListAllFileSearch.Items.Filter = CustomFilter;
        }

        protected override void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            base.BackgroundWorkerProgressChanged(sender, e);
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
                    "*.*", int.MinValue, int.MaxValue);
                _fileSearcher.FindFileList.ForEach(BackgroundWorkerSendProgress);
            });
        }

        protected override void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BackgroundWorkerDoWork(sender, e);
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