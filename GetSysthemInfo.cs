using System;
using System.Globalization;
using System.Management;
using System.Net;
using System.Windows.Forms;

namespace RCC
{
    public static class get_system_info
    {
        public static string get_cpu_name => get_single_component("Win32_Processor", "Name");
        public static string get_gpu_name => get_single_component("Win32_VideoController", "Name");
        public static string get_user_name => get_single_component("Win32_ComputerSystem", "Name");
        public static string get_screen_size => $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";
        public static string get_os_type => get_single_component("Win32_OperatingSystem", "Caption");
        public static string get_ram_size
        {
            get
            {
                all_dll_import.GetPhysicallyInstalledSystemMemory(out long size);
                return $"{size / 1024L / 1024L} Gb";
            }
        }
        public static string get_system_start_up => DateTime.Now.AddMilliseconds(-Environment.TickCount).ToString(CultureInfo.InvariantCulture);

        public static string get_user_external_ip()
        {
            string external_ip_string = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(external_ip_string).ToString();
        }

        public static string get_single_component(string hwclass, string syntax)
        {
            string result = string.Empty;
            ManagementObjectCollection mos = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM {hwclass}").Get();
            foreach (ManagementBaseObject base_object in mos)
            {
                ManagementObject management_object = (ManagementObject)base_object;
                if (Convert.ToString(management_object[syntax]) != "")
                    result = Convert.ToString(management_object[syntax]);
            }
            return result;
        }

    }
}
