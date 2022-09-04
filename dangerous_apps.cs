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
    
    public static class dangerous_apps
    {
        private readonly static List<string> list_dangerous_applications = new List<string>
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
                    RegistryKey sub_key = key.OpenSubKey(keyName);
                    if (sub_key == null) continue;
                    string display_name = sub_key.GetValue("DisplayName").ToString();
                    string install_path = sub_key.GetValue("InstallLocation").ToString();
                    bool is_dangerous = false;
                    foreach (string app in list_dangerous_applications)
                    {
                        if (display_name.ToLower().Contains(app.ToLower()))
                        {
                            display_name = app;
                            is_dangerous = true;
                            break;
                        }
                    }
                    if (is_dangerous)
                        result.Add(new dangerous_application(display_name,install_path));
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