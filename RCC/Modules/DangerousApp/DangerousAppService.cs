using System;
using System.Collections.Generic;
using Microsoft.Win32;
using RCC.windows;

namespace RCC.Modules.DangerousApp;

public class DangerousAppService : IDangerousApp<SDangerousApplication>
{
    public List<string> ListDangerousApplications { get; } = new()
    {
        "Process Hacker",
        "System Informer"
    };

    public List<SDangerousApplication> ListFindDangerousApplications { get; } = new();

    public List<string> ListRegistryKey { get; } = new()
    {
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    };

    public List<SDangerousApplication> FindAllApplicationInRegistry(in string registryKey)
    {
        if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
        List<SDangerousApplication> dangerousApplications = new();
        if (registryKey == string.Empty) throw new ArgumentException("RegistryKey is empty");
        var key = Registry.LocalMachine.OpenSubKey(registryKey) ??
                  throw new InvalidOperationException("Can't open registry SubKey");
        foreach (var keyName in key.GetSubKeyNames())
            try
            {
                var subKey = key.OpenSubKey(keyName) ??
                             throw new InvalidOperationException(
                                 $"Can't open key {keyName} because it is null");
                var displayName = subKey.GetValue("DisplayName", "").ToString();
                var installPath = subKey.GetValue("InstallLocation", "").ToString();
                var dangerousAppIndex = ListDangerousApplications.FindIndex(dangerousApp =>
                    displayName.ToLower().Contains(dangerousApp.ToLower()));
                if (dangerousAppIndex != -1)
                    dangerousApplications.Add(new SDangerousApplication(displayName, installPath));
            }
            catch (Exception e)
            {
                new MessageBox(
                        $"Пожалуйста перезапустите программу\nот имени админа.\nИли напишите нам в дискорд\nСообщение об ошибке:\n{e.Message}")
                    .Show();
            }

        key.Close();
        return dangerousApplications;
    }
}