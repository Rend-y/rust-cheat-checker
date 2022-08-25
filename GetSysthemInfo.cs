using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RCC
{
    public class GetSysthemInfo
    {
        public class usb_device_info
        {
            public string device_name { get; set; }
            public string description { get; set; }
            public string device_type { get; set; }
            public bool is_connect { get; set; }
            public string time_last_used { get; set; }
            public string creating_time { get; set; }

            public usb_device_info(string device_name, string description, string device_type, bool is_connect, string time_last_used, string creating_time)
            {
                this.device_name = device_name;
                this.description = description;
                this.device_type = device_type;
                this.is_connect = is_connect;
                this.time_last_used = time_last_used;
                this.creating_time = creating_time;
            }
        }


        public static string get_cpu_name => get_single_component("Win32_Processor", "Name");
        public static string get_gpu_name => get_single_component("Win32_VideoController", "Name");
        public static string get_uset_name => get_single_component("Win32_ComputerSystem", "Name");
        public static string get_screen_size => $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";
        public static string get_os_type => get_single_component("Win32_OperatingSystem", "Caption");
        public static string get_ram_size => $"{Int64.Parse(get_single_component("Win32_ComputerSystem", "TotalPhysicalMemory")) / 1048576 / 1024 + 1} Gb";
        public static string get_system_start_up => DateTime.Now.AddMilliseconds(-Environment.TickCount).ToString();


        public static string get_user_external_ip()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            return IPAddress.Parse(externalIpString).ToString();
        }

        public static List<usb_device_info> get_all_usb_device()
        {
            List<usb_device_info> result = new List<usb_device_info>();
            string path_to_local_application = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.ToString());
            string local_path_to_file = $"{path_to_local_application}\\USBDeview.exe";
            string path_to_save_usb_list = $"{path_to_local_application}\\usb_info.xml";
            string argument_to_startup = $"/sxml {path_to_save_usb_list}";
            File.WriteAllBytes(local_path_to_file, Properties.Resources.USBDeview);
            Process.Start(local_path_to_file, argument_to_startup).WaitForExit();

            Thread delete_thread = new Thread(remove_file_list =>
            {
                foreach (string file in (remove_file_list as string[]))
                {
                    Thread.Sleep(500);
                    File.Delete(file);
                }

            });
            delete_thread.Start(new string[] { local_path_to_file, path_to_save_usb_list });

            var load_xml_document = XDocument.Load(path_to_save_usb_list).Descendants("item");

            foreach (XElement element in load_xml_document)
            {
                string device_name = element.Element("device_name").Value;
                string description = element.Element("description").Value;
                string device_type = element.Element("device_type").Value;
                bool is_connect = element.Element("connected").Value == "Yes";
                string time_last_used = element.Element("last_plug_unplug_date").Value;
                string creating_time = element.Element("created_date").Value;
                usb_device_info device_info = new usb_device_info(device_name, description, device_type, is_connect, time_last_used, creating_time);
                result.Add(device_info);
            }

            return result;
        }

        public static string get_single_component(string hwclass, string syntax)
        {
            string result = string.Empty;
            ManagementObjectCollection mos = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM {hwclass}").Get();
            foreach (ManagementObject mj in mos)
            {
                if (Convert.ToString(mj[syntax]) != "")
                    result = Convert.ToString(mj[syntax]);
            }
            return result;
        }

    }
}
    