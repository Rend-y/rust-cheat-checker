﻿using System;
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
            new MessageBox("Была запушенна проверка консольный комманд\nСтарт через 10 секунд").Show();
            Thread.Sleep(10000);
            this._consoleCommandList.ForEach(element =>
            {
                Clipboard.SetText(element);
                Keyboard.PasteEvent().SendEvent();
                new Keyboard(KeyboardKeys.Return).SendEvent();
                Clipboard.Clear();
            });
            new MessageBox("Проверка консольных команд успешно завершена").Show();
        }
    }
}