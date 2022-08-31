using System;
using System.Runtime.InteropServices;
using static RCC.glass_effect;

namespace RCC
{
    public static class all_dll_import
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        [DllImport("gdi32.dll")]
        public static extern int AddFontResource(string lpFilename);
    }
}
