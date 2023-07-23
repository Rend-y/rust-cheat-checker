using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            SteamData steam = e.UserState as SteamData;
            
            if (steam == null)
                return;
            
            ListOtherAccounts.Items.Add(new SteamData(steam.Username, steam.SteamId, steam.AccountLevel, steam.AvatarUrl, steam.IsHideAccount, steam.IsDeleted));
        }
        
        void BackgroundWorkerFindSteamAccountDoWork(object sender, DoWorkEventArgs e)
        {
            string steamPathToLoginUser = LocalInfo.GetPathToLoginUser;
            string steamPathToConfig = LocalInfo.GetPathToConfig;
            
            if (!File.Exists(steamPathToLoginUser) && !File.Exists(steamPathToConfig))
                return;

            
            string loginUserFileData = File.ReadAllText(steamPathToLoginUser);
            string configFileData = File.ReadAllText(steamPathToConfig);
            List<string> getLoginUserSteamId = LocalInfo.GetSteamIsFromContent(loginUserFileData);
            List<string> getConfigSteamId = LocalInfo.GetSteamIsFromContent(configFileData);
            List<string> getCoPlaySteamId = LocalInfo.GetSteamIdFromCoPlay();

            List<string> groupListSteamId = new List<string>();
            groupListSteamId.AddRange(getLoginUserSteamId);
            groupListSteamId.AddRange(getConfigSteamId);
            groupListSteamId.AddRange(getCoPlaySteamId);
            groupListSteamId.Sort();

            List<string> isDeletedAccount = new List<string>();
            List<string> normalAccount = new List<string>();

            normalAccount.AddRange(getLoginUserSteamId.Intersect(getConfigSteamId).ToList());
            isDeletedAccount.AddRange(getLoginUserSteamId.Except(getConfigSteamId).ToList());
            normalAccount.AddRange(getLoginUserSteamId.Intersect(getCoPlaySteamId).ToList());
            isDeletedAccount.AddRange(getLoginUserSteamId.Except(getCoPlaySteamId).ToList());
            
            normalAccount.AddRange(getConfigSteamId.Intersect(getLoginUserSteamId).ToList());
            isDeletedAccount.AddRange(getConfigSteamId.Except(getLoginUserSteamId).ToList());
            normalAccount.AddRange(getConfigSteamId.Intersect(getCoPlaySteamId).ToList());
            isDeletedAccount.AddRange(getConfigSteamId.Except(getCoPlaySteamId).ToList());
            
            normalAccount.AddRange(getCoPlaySteamId.Intersect(getConfigSteamId).ToList());
            isDeletedAccount.AddRange(getCoPlaySteamId.Except(getConfigSteamId).ToList());
            normalAccount.AddRange(getCoPlaySteamId.Intersect(getLoginUserSteamId).ToList());
            isDeletedAccount.AddRange(getCoPlaySteamId.Except(getLoginUserSteamId).ToList());
            
            isDeletedAccount = isDeletedAccount.Distinct().ToList();
            normalAccount = normalAccount.Except(isDeletedAccount).Distinct().ToList();

            int i = 0;
            normalAccount.ForEach(steam_id =>
            {
                backgroundWorkerFindSteamAccount.ReportProgress(i, LocalInfo.PeParseFromSteam(long.Parse(steam_id), false));
                i++;
            });
            isDeletedAccount.ForEach(steam_id =>
            {
                backgroundWorkerFindSteamAccount.ReportProgress(i, LocalInfo.PeParseFromSteam(long.Parse(steam_id), true));
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
            
            LabelFullPathToSteam.Content = LocalInfo.GetSteamLocation();
            
            SteamData lastAccountInfo = LocalInfo.GetLastAccountInfo();

            LabelSteamAccountSteamId.Content = lastAccountInfo.GetSteamId;
            LabelSteamAccountUsername.Content = lastAccountInfo.GetUsername;
            LabelCpuType.Content = GetSystemInfo.GetCpuName;
            LabelGpuType.Content = GetSystemInfo.GetGpuName;
            LabelScreenSize.Content = GetSystemInfo.GetScreenSize;
            LabelWindowsType.Content = GetSystemInfo.GetOsType;
            LabelMemorySize.Content = GetSystemInfo.GetRamSize;
            LabelStartUpTime.Content = GetSystemInfo.GetSystemStartUp;
            LabelUserIp.Content = GetSystemInfo.get_user_external_ip();
            
            ImageBrush myBrush = new ImageBrush
            {
                ImageSource = lastAccountInfo.GetAccountAvatar
            };
            RectangleLocalProfileImage.Fill = myBrush;
        }

        private void ListOtherAccounts_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => 
            Process.Start($"https://steamcommunity.com/profiles/{((SteamData)ListOtherAccounts.SelectedItem).SteamId}");

        private void ButtonOpenSteamPath_OnClick(object sender, RoutedEventArgs e) => Process.Start(LabelFullPathToSteam.Content.ToString());
    }
}