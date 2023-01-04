using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace RCC
{
    public class dangerous_application
    {
        public string display_name { get; set; }
        public string install_path { get; set; }
        public dangerous_application(string display_name, string install_path)
        {
            this.display_name = display_name;
            this.install_path = install_path;
        }
    }
    
    public static class DangerousApps
    {
        private static readonly List<string> ListDangerousApplications = new List<string>
        {
            "Process Hacker",
        };

        public static List<dangerous_application> start_scan()
        {
            List<dangerous_application> result = new List<dangerous_application>();
            
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
                        result.Add(new dangerous_application(displayName,installPath));
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