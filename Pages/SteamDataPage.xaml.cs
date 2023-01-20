using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RCC.Steam;

namespace RCC.Pages
{
    public partial class SteamDataPage : Page
    {
        #region Background Worker Functions
        void BackgroundWorkerFindSteamAccountProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            steam_data steam = e.UserState as steam_data;
            
            if (steam == null)
                return;
            
            ListOtherAccounts.Items.Add(new steam_data(steam.username, steam.steam_id, steam.account_level, steam.avatar_url, steam.is_hide_account));
        }
        
        void BackgroundWorkerFindSteamAccountDoWork(object sender, DoWorkEventArgs e)
        {
            string steamPathToLoginUser = local_info.GetPathToLoginUser();
            if (!File.Exists(steamPathToLoginUser))
                return;
            string fileData = File.ReadAllText(steamPathToLoginUser);
            List<string> getSteamIdData = local_info.GetAllSteamId(fileData);

            int i = 0;
            getSteamIdData.ForEach(steam_id =>
            {
                backgroundWorkerFindSteamAccount.ReportProgress(i, local_info.PeParseFromSteam(long.Parse(steam_id)));
                i++;
            });
        }
        #endregion

        private readonly BackgroundWorker backgroundWorkerFindSteamAccount = new BackgroundWorker();
        public SteamDataPage()
        {
            backgroundWorkerFindSteamAccount.DoWork += BackgroundWorkerFindSteamAccountDoWork;
            backgroundWorkerFindSteamAccount.ProgressChanged += BackgroundWorkerFindSteamAccountProgressChanged;
            backgroundWorkerFindSteamAccount.WorkerReportsProgress = true;
            backgroundWorkerFindSteamAccount.RunWorkerAsync();
            InitializeComponent();
            
            LabelFullPathToSteam.Content = local_info.GetSteamLocation();
            
            steam_data lastAccountInfo = local_info.GetLastAccountInfo();

            LabelSteamAccountSteamId.Content = lastAccountInfo.get_steam_id;
            LabelSteamAccountUsername.Content = lastAccountInfo.get_username;
            LabelCpuType.Content = GetSystemInfo.GetCpuName;
            LabelGpuType.Content = GetSystemInfo.GetGpuName;
            LabelScreenSize.Content = GetSystemInfo.GetScreenSize;
            LabelWindowsType.Content = GetSystemInfo.GetOsType;
            LabelMemorySize.Content = GetSystemInfo.GetRamSize;
            LabelStartUpTime.Content = GetSystemInfo.GetSystemStartUp;
            LabelUserIp.Content = GetSystemInfo.get_user_external_ip();
            
            ImageBrush myBrush = new ImageBrush
            {
                ImageSource = lastAccountInfo.get_account_avatar
            };
            RectangleLocalProfileImage.Fill = myBrush;
        }

        private void ListOtherAccounts_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => 
            Process.Start($"https://steamcommunity.com/profiles/{((steam_data)ListOtherAccounts.SelectedItem).steam_id}");

        private void ButtonOpenSteamPath_OnClick(object sender, RoutedEventArgs e) => Process.Start(LabelFullPathToSteam.Content.ToString());
    }
}