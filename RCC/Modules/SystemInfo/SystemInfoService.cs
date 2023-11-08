using System;
using System.Globalization;
using System.Management;
using System.Net;
using System.Windows.Forms;

namespace RCC.Modules.SystemInfo;

public class SystemInfoService : ISystemInfo
{
    public SystemInfoService()
    {
        CpuName = GetSingleComponentInfo("Win32_Processor", "Name");
        GpuName = GetSingleComponentInfo("Win32_VideoController", "Name");
        UserName = GetSingleComponentInfo("Win32_ComputerSystem", "Name");
        ScreenSize = $"{Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}";
        OsType = GetSingleComponentInfo("Win32_OperatingSystem", "Caption");

        AllDllImport.GetPhysicallyInstalledSystemMemory(out var size);
        RamSize = $"{size / 1024L / 1024L} Gb";
        SystemStartUpTime = DateTime.Now.AddMilliseconds(-Environment.TickCount).ToString(CultureInfo.InvariantCulture);
        ;

        var externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "")
            .Replace("\\n", "").Trim();
        ExternalIpAddress = IPAddress.Parse(externalIpString).ToString();
    }

    public string CpuName { get; }
    public string GpuName { get; }
    public string UserName { get; }
    public string ScreenSize { get; }
    public string OsType { get; }
    public string RamSize { get; }
    public string SystemStartUpTime { get; }
    public string ExternalIpAddress { get; }

    public string GetSingleComponentInfo(in string hwClass, in string syntax)
    {
        var result = string.Empty;
        var mos = new ManagementObjectSearcher("root\\CIMV2", $"SELECT * FROM {hwClass}").Get();
        foreach (var baseObject in mos)
        {
            var managementObject = (ManagementObject)baseObject;
            if (Convert.ToString(managementObject[syntax]) != "")
                result = Convert.ToString(managementObject[syntax]);
        }

        return result;
    }
}