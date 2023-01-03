using System;
using System.Globalization;
using System.Management;
using System.Net;
using System.Windows.Forms;

namespace RCC
{
    public static class GetSystemInfo
    {
        public static string GetCpuName => get_single_component("Win32_Processor", "Name");
        public static string GetGpuName => get_single_component("Win32_VideoController", "Name");
        public static string GetUserName => get_single_component("Win32_ComputerSystem", "Name");
        public static string GetScreenSize => $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";
        public static string GetOsType => get_single_component("Win32_OperatingSystem", "Caption");
        public static string GetRamSize
        {
            get
            {
                AllDllImport.GetPhysicallyInstalledSystemMemory(out long size);
                return $"{size / 1024L / 1024L} Gb";
            }
        }
        public static string GetSystemStartUp => DateTime.Now.AddMilliseconds(-Environment.TickCount).ToString(CultureInfo.InvariantCulture);

        public static string get_user_external_ip()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(externalIpString).ToString();
        }

        private static string get_single_component(string hwclass, string syntax)
        {
            string result = string.Empty;
            ManagementObjectCollection mos = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM {hwclass}").Get();
            foreach (ManagementBaseObject baseObject in mos)
            {
                ManagementObject managementObject = (ManagementObject)baseObject;
                if (Convert.ToString(managementObject[syntax]) != "")
                    result = Convert.ToString(managementObject[syntax]);
            }
            return result;
        }

    }
}
