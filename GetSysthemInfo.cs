using System;
using System.Management;
using System.Net;
using System.Windows.Forms;

namespace RCC
{
    public class GetSysthemInfo
    {

        public static string get_cpu_name => GetComponent("Win32_Processor", "Name");
        public static string get_gpu_name => GetComponent("Win32_VideoController", "Name");
        public static string get_uset_name => GetComponent("Win32_ComputerSystem", "Name");
        public static string get_screen_size => $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";
        public static string get_os_type => GetComponent("Win32_OperatingSystem", "Caption");
        public static string get_ram_size => $"{Int64.Parse(GetComponent("Win32_ComputerSystem", "TotalPhysicalMemory")) / 1048576 / 1024 + 1} Gb";
        public static string get_system_start_up => DateTime.Now.AddMilliseconds(-Environment.TickCount).ToString();

        public static string get_user_external_ip()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(externalIpString).ToString();
        }

        public static string GetComponent(string hwclass, string syntax)
        {
            string result = string.Empty;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM {hwclass}");
            foreach (ManagementObject mj in mos.Get())
            {
                if (Convert.ToString(mj[syntax]) != "")
                    result = Convert.ToString(mj[syntax]);
            }
            return result;
        }

    }
}
    