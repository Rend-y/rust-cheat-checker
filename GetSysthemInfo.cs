using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Windows;
using System.Windows.Forms;

namespace RCC
{
    public class GetSysthemInfo
    {

        public static string get_cpu_name => GetComponent("Win32_Processor", "Name");
        public static string get_gpu_name => GetComponent("Win32_VideoController", "Name");
        public static string get_uset_name => GetComponent("Win32_ComputerSystem", "Name");

        public static string get_screen_size => $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";

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
    