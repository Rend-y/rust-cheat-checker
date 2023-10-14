namespace RCC.Modules.SystemInfo;

public interface ISystemInfo
{
    public string CpuName { get; }
    public string GpuName { get; }
    public string UserName { get; }
    public string ScreenSize { get; }
    public string OsType { get; }
    public string RamSize { get; }
    public string SystemStartUpTime { get; }
    public string ExternalIpAddress { get; }
    public string GetSingleComponentInfo(in string hwClass, in string syntax);
}