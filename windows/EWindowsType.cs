using System;
using System.ComponentModel;

namespace RCC.windows;

[Flags]
public enum EWindowsType
{
    [Description("")]
    Warning,
    [Description("")]
    Error,
    [Description("")]
    Success,
    [Description("")]
    Any,
}