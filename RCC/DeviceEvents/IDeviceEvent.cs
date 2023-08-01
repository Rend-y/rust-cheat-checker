using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RCC.DeviceEvents
{
    public interface IDeviceEvent<T>
    {
        List<T> Keys { get; }
        int Interval { get; }
        void SendEvent();
    }
}