using System.Collections.Generic;

namespace RCC.Modules.AutoCheck.ConsoleCommand;

public interface IConsoleCommand
{
    List<string> ConsoleCommandList { get; }
    void Run();
}