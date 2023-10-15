using System;
using System.Threading;

namespace RCC.Modules.Device.MouseEvent
{
    [Flags]
    public enum MouseEventFlags
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010,
        XDown = 0x00000080,
        Xup = 0x00000100
    }

    public class MouseEvent : IMouseEvent<MouseEventFlags>
    {
        public void CreateEvent(int posX, int posY, MouseEventFlags eventFlags, int interval = 0)
        {
            AllDllImport.mouse_event((uint)eventFlags, (uint)posX, (uint)posY, 0, 0);
            Thread.Sleep(interval != 0 ? interval : 100);
            AllDllImport.mouse_event((uint)eventFlags, (uint)posX, (uint)posY, 0, 0);
        }
    }
}