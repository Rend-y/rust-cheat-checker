using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

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

    public partial class LastActivityPage : APage
    {
        public LastActivityPage(ILogger<LastActivityPage> logger) : base(logger)
        {
            InitializeComponent();
            RunBackgroundWorker();
        }

        protected override void BackgroundWorkerDoWork()
        {
            base.BackgroundWorkerDoWork();
            string localPathToFile = $"{Utilities.PathToLocalApplication}\\LastActivityView.exe";
            string pathToSaveUsbList = $"{Utilities.PathToLocalApplication}\\last_activity_view.xml";
            var loadXmlDocument = Utilities.GetXmlDocumentFromWebProcess(localPathToFile,
                "https://github.com/Midoruya/rust-cheat-checker/blob/main/RCC/Resources/LastActivityView.exe?raw=true",
                pathToSaveUsbList);

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
                BackgroundWorkerSendProgress(info);
            }
        }

        protected override async void BackgroundWorkerProgressChanged(object sender)
        {
            base.BackgroundWorkerProgressChanged(sender);
            var lastActivityInfo = sender as LastActivityInfo;

            if (lastActivityInfo == null)
                return;

            ListAllLastActivity.Dispatcher.Invoke(() =>
            {
                ListAllLastActivity.Items.Add(new LastActivityInfo(lastActivityInfo.ActionTime,
                    lastActivityInfo.Description, lastActivityInfo.Filename, lastActivityInfo.FullPath));
            });
        }
    }
}