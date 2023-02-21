using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using RCC.DeviceEvents;
using MessageBox = RCC.windows.MessageBox;

namespace RCC.QuickCheck
{
    public class ConsoleCommand
    {
        private readonly List<string> _consoleCommandList = new List<string>()
        {
            "debug.debugcamera",
        };
        public ConsoleCommand()
        {
            MessageBox.Show("Была запушенна проверка консольный комманд\nСтарт через 10 секунд");
            Thread.Sleep(10000);
            this._consoleCommandList.ForEach(element =>
            {
                Clipboard.SetText(element);
                Keyboard.PasteEvent().SendKeyEvent();
                new Keyboard(KeyboardKeys.Return).SendKeyEvent();
                Clipboard.Clear();
            });
            MessageBox.Show("Проверка консольных команд успешно завершена");
        }
    }
}