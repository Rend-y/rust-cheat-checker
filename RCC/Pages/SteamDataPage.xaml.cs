using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using RCC.Modules.SteamInformation;
using RCC.Modules.SystemInfo;

namespace RCC.Pages
{
    public partial class SteamDataPage : APage
    {
        private readonly ISteamInformation<SteamData> _steamInformation;

        public SteamDataPage(ILogger<SteamDataPage> logger, ISteamInformation<SteamData> steamInformation, ISystemInfo systemInfo) :
            base(logger)
        {
            InitializeComponent();
            RunBackgroundWorker();
            _steamInformation = steamInformation;
            LabelFullPathToSteam.Content = _steamInformation.GetSteamLocation();
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

        protected override void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            base.BackgroundWorkerProgressChanged(sender, e);
            try
            {
                var steam = e.UserState as Tuple<long, bool> ?? throw new InvalidOperationException("SteamData is null");
                Logger.LogInformation("Background worker find steam account get new value: {SteamId}", steam.Item1);
                ListOtherAccounts.Items.Add(new SteamData(steam.Item1, steam.Item2));
            }
            catch (Exception exception)
            {
                Logger.LogTrace(exception, "BackgroundWorkerFindSteamAccountProgressChanged");
            }
        }

        protected override void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BackgroundWorkerDoWork(sender, e);
            var steamPathToLoginUser = _steamInformation.PathToLoginData;
            var steamPathToConfig = _steamInformation.PathToSteamConfig;
            if (!File.Exists(steamPathToLoginUser) && !File.Exists(steamPathToConfig))
            {
                Logger.LogError("steam paths not exist");
                return;
            }
            string loginUserFileData = File.ReadAllText(steamPathToLoginUser);
            string configFileData = File.ReadAllText(steamPathToConfig);
            var getLoginUserSteamId = _steamInformation.GetSteamIdFromContent(loginUserFileData);
            var getConfigSteamId = _steamInformation.GetSteamIdFromContent(configFileData);
            var deletedAccount = new List<string>();
            List<string> normalAccount = new List<string>();

            normalAccount.AddRange(getConfigSteamId);
            normalAccount.AddRange(getLoginUserSteamId);
            normalAccount.AddRange(getLoginUserSteamId.Intersect(getConfigSteamId).ToList());
            deletedAccount.AddRange(getLoginUserSteamId.Except(getConfigSteamId).ToList());

            normalAccount.AddRange(getConfigSteamId.Intersect(getLoginUserSteamId).ToList());
            deletedAccount.AddRange(getConfigSteamId.Except(getLoginUserSteamId).ToList());

            deletedAccount = deletedAccount.Distinct().ToList();
            normalAccount = normalAccount.Except(deletedAccount).Distinct().ToList();

            normalAccount.ForEach(steamId => { BackgroundWorkerSendProgress(new Tuple<long, bool>(long.Parse(steamId), false)); });
            deletedAccount.ForEach(steamId => { BackgroundWorkerSendProgress(new Tuple<long, bool>(long.Parse(steamId), true)); });
        }

        #endregion
    }
}