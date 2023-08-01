using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using MessageBox = RCC.windows.MessageBox;

namespace RCC
{
    public struct DangerousApplication
    {
        public string DisplayName { get; set; }
        public string InstallPath { get; set; }
        public DangerousApplication(string displayName, string installPath)
        {
            this.DisplayName = displayName;
            this.InstallPath = installPath;
        }
    }
    
    public class DangerousApps
    {
        private static readonly List<string> ListDangerousApplications = new List<string>
        {
            "Process Hacker",
        };

        private readonly List<DangerousApplication> _dangerousApplications = new List<DangerousApplication>();
        public DangerousApps()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key == null)
            {
                new MessageBox("Пожалуйста запустите программу\nот имени администрартора").Show();
                return;
            }
            foreach (string keyName in key.GetSubKeyNames())
            {
                try 
                {
                    RegistryKey subKey = key.OpenSubKey(keyName);
                    if (subKey == null) continue;
                    string displayName = subKey.GetValue("DisplayName", "").ToString();
                    string installPath = subKey.GetValue("InstallLocation", "").ToString();
                    int dangerousAppIndex = ListDangerousApplications.ToList()
                        .FindIndex(dangerousApp => displayName.ToLower().Contains(dangerousApp.ToLower()));
                    if (dangerousAppIndex != -1)
                        this._dangerousApplications.Add(new DangerousApplication(displayName,installPath));
                }
                catch (Exception e)
                {
                    new MessageBox($"Пожалуйста перезапустите программу\nот имени админа.\nИли напишите нам в дискорд\nСообщение об ошибке:\n{e.Message}").Show();
                    return;
                }
            }
            key.Close();
        }
        
        public List<DangerousApplication> AllFindDangerousApplications() => _dangerousApplications;
    }
}