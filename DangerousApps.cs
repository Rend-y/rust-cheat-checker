using System;
using System.Collections.Generic;
using Microsoft.Win32;

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
    
    public static class DangerousApps
    {
        private static readonly List<string> ListDangerousApplications = new List<string>
        {
            "Process Hacker",
        };

        public static List<DangerousApplication> start_scan()
        {
            List<DangerousApplication> result = new List<DangerousApplication>();
            
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                try 
                {
                    RegistryKey subKey = key.OpenSubKey(keyName);
                    if (subKey == null) continue;
                    string displayName = subKey.GetValue("DisplayName").ToString();
                    string installPath = subKey.GetValue("InstallLocation").ToString();
                    bool isDangerous = false;
                    foreach (string app in ListDangerousApplications)
                    {
                        if (displayName.ToLower().Contains(app.ToLower()))
                        {
                            displayName = app;
                            isDangerous = true;
                            break;
                        }
                    }
                    if (isDangerous)
                        result.Add(new DangerousApplication(displayName,installPath));
                }
                catch
                { 
                    // ignored
                }
            }
            
            return result;
        }
    }
}