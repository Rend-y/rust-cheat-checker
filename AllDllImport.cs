using System;
using System.Runtime.InteropServices;
using static RCC.glass_effect;

namespace RCC
{
    public static class AllDllImport
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    }
}
