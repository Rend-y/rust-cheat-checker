using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RCC.Modules.SteamInformation;
using RCC.Modules.SystemInfo;

namespace RCC.Pages
{
    public partial class SteamDataPage : Page
    {
        private readonly BackgroundWorker _backgroundWorkerFindSteamAccount = new();
        private readonly ISteamInformation<SteamData> _steamInformation;

        public SteamDataPage(ISteamInformation<SteamData> steamInformation, ISystemInfo systemInfo)
        {
            InitializeComponent();

            _steamInformation = steamInformation;
            LabelFullPathToSteam.Content = _steamInformation.GetSteamLocation();

            _backgroundWorkerFindSteamAccount.DoWork += BackgroundWorkerFindSteamAccountDoWork;
            _backgroundWorkerFindSteamAccount.ProgressChanged += BackgroundWorkerFindSteamAccountProgressChanged;
            _backgroundWorkerFindSteamAccount.WorkerReportsProgress = true;
            _backgroundWorkerFindSteamAccount.RunWorkerAsync();

            var lastAccountInfo = _steamInformation.GetLastSteamAccountInfo();

            LabelSteamAccountSteamId.Content = lastAccountInfo.GetSteamId;
            LabelSteamAccountUsername.Content = lastAccountInfo.GetUsername;
            LabelCpuType.Content = systemInfo.CpuName;
            LabelGpuType.Content = systemInfo.GpuName;
            LabelScreenSize.Content = systemInfo.ScreenSize;
            LabelWindowsType.Content = systemInfo.OsType;
            LabelMemorySize.Content = systemInfo.RamSize;
            LabelStartUpTime.Content = systemInfo.SystemStartUpTime;
            LabelUserIp.Content = systemInfo.ExternalIpAddress;

            ImageBrush myBrush = new ImageBrush
            {
                ImageSource = lastAccountInfo.GetAccountAvatar
            };
            RectangleLocalProfileImage.Fill = myBrush;
        }

        private void ListOtherAccounts_OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            Process.Start($"https://steamcommunity.com/profiles/{((SteamData)ListOtherAccounts.SelectedItem).SteamId}");

        private void ButtonOpenSteamPath_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start(LabelFullPathToSteam.Content.ToString());

        #region Background Worker Functions

        void BackgroundWorkerFindSteamAccountProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var steam = e.UserState as SteamData ?? throw new InvalidOperationException("SteamData is null");
            ListOtherAccounts.Items.Add(new SteamData(steam.Username, steam.SteamId, steam.AccountLevel,
                steam.AvatarUrl, steam.IsHideAccount, steam.IsDeleted));
        }

        void BackgroundWorkerFindSteamAccountDoWork(object sender, DoWorkEventArgs e)
        {
            var steamPathToLoginUser = _steamInformation.PathToLoginData;
            var steamPathToConfig = _steamInformation.PathToSteamConfig;

            if (!File.Exists(steamPathToLoginUser) && !File.Exists(steamPathToConfig))
                return;


            string loginUserFileData = File.ReadAllText(steamPathToLoginUser);
            string configFileData = File.ReadAllText(steamPathToConfig);
            var getLoginUserSteamId = _steamInformation.GetSteamIdFromContent(loginUserFileData);
            var getConfigSteamId = _steamInformation.GetSteamIdFromContent(configFileData);
            var getCoPlaySteamId = _steamInformation.GetSteamIdFromCoPlayData();

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
            normalAccount.ForEach(steamId =>
            {
                _backgroundWorkerFindSteamAccount.ReportProgress(i,
                    _steamInformation.GetSteamData(long.Parse(steamId)));
                i++;
            });
            isDeletedAccount.ForEach(steamId =>
            {
                _backgroundWorkerFindSteamAccount.ReportProgress(i,
                    _steamInformation.GetSteamData(long.Parse(steamId), true));
                i++;
            });
        }

        #endregion
    }
}