using System;
using System.ComponentModel;

namespace RCC.Windows;

[Flags]
public enum EWindowsType
{
    [Description("")] Warning,
    [Description("")] Error,
    [Description("")] Success,
    [Description("")] Any,
}