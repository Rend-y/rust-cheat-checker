using System;
using System.Collections.Generic;
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
    public class Mouse : IDeviceEvent<MouseEventFlags>
    {
        private readonly int _posX;
        private readonly int _posY;
        public List<MouseEventFlags> Keys { get; }
        public int Interval { get; }
        Mouse(int posX, int posY, MouseEventFlags eventFlags, int interval = 0)
        {
            this._posX = posX;
            this._posY = posY;
            this.Keys = new List<MouseEventFlags>() {eventFlags};
            this.Interval = interval;
        }
        public void SendEvent()
        {
            if (Keys?[0] == null) throw new ArgumentException("Mouse key does not contains value by index zero");
            AllDllImport.mouse_event((uint)Keys[0], (uint)_posX, (uint)_posY, 0, 0);
            Thread.Sleep(Interval != 0 ? Interval : 100);
            AllDllImport.mouse_event((uint)Keys[0], (uint)_posX, (uint)_posY, 0, 0);
        }
    }
}