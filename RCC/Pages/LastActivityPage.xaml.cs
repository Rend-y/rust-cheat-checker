using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RCC.Pages
{
    public class LastActivityInfo
    {
        public LastActivityInfo(string action_time, string description, string filename, string full_path)
        {
            ActionTime = action_time;
            Description = description;
            Filename = filename;
            FullPath = full_path;
        }

        public LastActivityInfo()
        {
            throw new NotImplementedException();
        }

        public string ActionTime { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public string FullPath { get; set; }
    }

    public partial class LastActivityPage : Page
    {
        private readonly BackgroundWorker backgroundWorkerFindLastActivity = new BackgroundWorker();

        public LastActivityPage()
        {
            InitializeComponent();
            backgroundWorkerFindLastActivity.DoWork += BackgroundWorkerFindLastActivityDoWork;
            backgroundWorkerFindLastActivity.ProgressChanged += BackgroundWorkerFindLastActivityProgressChanged;
            backgroundWorkerFindLastActivity.WorkerReportsProgress = true;
            backgroundWorkerFindLastActivity.RunWorkerAsync();
        }

        void BackgroundWorkerFindLastActivityDoWork(object sender, DoWorkEventArgs e)
        {
            string localPathToFile = $"{Utilities.PathToLocalApplication}\\LastActivityView.exe";
            string pathToSaveUsbList = $"{Utilities.PathToLocalApplication}\\last_activity_view.xml";
            var loadXmlDocument = Utilities.GetXmlDocumentFromWebProcess(localPathToFile,
                "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/LastActivityView.exe?raw=true",
                pathToSaveUsbList);

            int i = 0;
            foreach (XElement element in loadXmlDocument)
            {
                string actionTime = element.Element("action_time")?.Value;
                string description = element.Element("description")?.Value;
                string filename = element.Element("filename")?.Value;
                string fullPath = element.Element("full_path")?.Value;

                if (!File.Exists(fullPath))
                    fullPath = "File has been removed";

                if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(fullPath))
                    continue;

                LastActivityInfo info = new LastActivityInfo(actionTime, description, filename, fullPath);
                backgroundWorkerFindLastActivity.ReportProgress(i, info);
                i++;
            }
        }

        void BackgroundWorkerFindLastActivityProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LastActivityInfo lastActivityInfo = e.UserState as LastActivityInfo;

            if (lastActivityInfo == null)
                return;

            ListAllLastActivity.Items.Add(new LastActivityInfo(lastActivityInfo.ActionTime,
                lastActivityInfo.Description, lastActivityInfo.Filename, lastActivityInfo.FullPath));
        }
    }
}