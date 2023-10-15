using System.Collections.Generic;
using System.Threading;
using System.Windows;
using RCC.Modules.Device.KeyboardEvent;
using MessageBox = RCC.Windows.MessageBox;

namespace RCC.Modules.AutoCheck.ConsoleCommand
{
    public class ConsoleCommand : IConsoleCommand
    {
        private readonly IKeyboardEvent<KeyboardKeys> _keyboardEvent;

        public ConsoleCommand(IKeyboardEvent<KeyboardKeys> keyboardEvent)
        {
            _keyboardEvent = keyboardEvent;
            ConsoleCommandList = new List<string>
            {
                "debug.debugcamera"
            };
        }

        public List<string> ConsoleCommandList { get; }

        public void Run()
        {
            new MessageBox("Была запушенна проверка консольный комманд\nСтарт через 10 секунд").Show();
            Thread.Sleep(10000);
            ConsoleCommandList.ForEach(element =>
            {
                Clipboard.SetText(element);
                _keyboardEvent.PasteEvent();
                _keyboardEvent.SendEvent(KeyboardKeys.Return);
                Clipboard.Clear();
            });
            new MessageBox("Проверка консольных команд успешно завершена").Show();
        }
    }
}