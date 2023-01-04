using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RCC.Pages
{
    public partial class LastActivityPage : Page
    {
        void BackgroundWorkerFindLastActivityDoWork(object sender, DoWorkEventArgs e)
        {
            string localPathToFile = $"{Utilities.PathToLocalApplication}\\LastActivityView.exe";
            string pathToSaveUsbList = $"{Utilities.PathToLocalApplication}\\last_activity_view.xml";
            var loadXmlDocument = Utilities.GetXmlDocumentFromWebProcess(localPathToFile, "https://github.com/Midoruya/rust-cheat-checker/blob/main/Resources/LastActivityView.exe?raw=true", pathToSaveUsbList);

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

                main_window.LastActivityInfo info = new main_window.LastActivityInfo(actionTime, description, filename, fullPath);
                backgroundWorkerFindLastActivity.ReportProgress(i, info);
                i++;
            }
        }
        
        void BackgroundWorkerFindLastActivityProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            main_window.LastActivityInfo lastActivityInfo = e.UserState as main_window.LastActivityInfo;
            
            if (lastActivityInfo == null)
                return;
            
            ListAllLastActivity.Items.Add(new main_window.LastActivityInfo(lastActivityInfo.action_time, lastActivityInfo.description, lastActivityInfo.filename, lastActivityInfo.full_path));
        }
        
        private readonly BackgroundWorker backgroundWorkerFindLastActivity = new BackgroundWorker();

        public LastActivityPage()
        {
            InitializeComponent();
            backgroundWorkerFindLastActivity.DoWork += BackgroundWorkerFindLastActivityDoWork;
            backgroundWorkerFindLastActivity.ProgressChanged += BackgroundWorkerFindLastActivityProgressChanged;
            backgroundWorkerFindLastActivity.WorkerReportsProgress = true;
            backgroundWorkerFindLastActivity.RunWorkerAsync();
        }
    }
}