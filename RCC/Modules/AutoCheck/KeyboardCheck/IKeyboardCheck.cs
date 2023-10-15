using System.Collections.Generic;
using RCC.Modules.Device.KeyboardEvent;

namespace RCC.Modules.AutoCheck.KeyboardCheck;

public interface IKeyboardCheck
{
    List<List<KeyboardKeys>> CheckKeyList { get; }
    void Run();
}