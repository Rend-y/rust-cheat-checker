using System;
using System.Threading;

namespace RCC.DeviceEvents
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
    public class Mouse
    {
        private readonly int _posX;
        private readonly int _posY;
        private readonly MouseEventFlags _eventFlags;
        private readonly int _interval;
        Mouse(int posX, int posY, MouseEventFlags eventFlags, int interval = 0)
        {
            this._posX = posX;
            this._posY = posY;
            this._eventFlags = eventFlags;
            this._interval = interval;
        }
        public void SendMouseEvent()
        {
            AllDllImport.mouse_event((uint)_eventFlags, (uint)_posX, (uint)_posY, 0, 0);
            Thread.Sleep(_interval != 0 ? _interval : 100);
            AllDllImport.mouse_event((uint)_eventFlags, (uint)_posX, (uint)_posY, 0, 0);
        }
    }
}