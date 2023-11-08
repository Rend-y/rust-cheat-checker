using System.Collections.Generic;

namespace RCC.Modules.Device.KeyboardEvent;

public interface IKeyboardEvent<T>
{
    void SendEvent(T key, int interval = 0);
    void SendEvent(List<T> keys, int interval = 0);
    void PasteEvent();
}