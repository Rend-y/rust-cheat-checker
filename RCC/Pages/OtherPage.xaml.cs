using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using RCC.Modules.AutoCheck.ConsoleCommand;
using RCC.Modules.AutoCheck.KeyboardCheck;
using RCC.Modules.DangerousApp;
using RCC.Windows;

namespace RCC.Pages
{
    public class KeyBind
    {
        public KeyBind(string bind)
        {
            this.Bind = bind;
        }

        public string Bind { get; set; }
    }

    public partial class OtherPage : APage
    {
        private readonly IConsoleCommand _consoleCommand;
        private readonly IKeyboardCheck _keyboardCheck;

        public OtherPage(IDangerousApp<SDangerousApplication> dangerousApps, IKeyboardCheck keyboardCheck,
            IConsoleCommand consoleCommand, ILogger<OtherPage> logger) : base(logger)
        {
            InitializeComponent();
            _keyboardCheck = keyboardCheck;
            _consoleCommand = consoleCommand;
            dangerousApps.ListFindDangerousApplications.ForEach(item => ListAllDangerousApps.Items.Add(item));
            this.GetAllKeyBind();
        }

        private void GetAllKeyBind()
        {
            try
            {
                Process rustProcesses = Process.GetProcessesByName("RustClient")[0];
                if (rustProcesses.MainModule == null) return;
                List<string> pathArray = rustProcesses.MainModule.FileName.Split('\\').ToList();
                pathArray.RemoveAt(pathArray.Count - 1);
                string pathToKeys = $"{string.Join("\\", pathArray)}\\cfg\\keys.cfg";
                string[] fileContent = File.ReadAllLines(pathToKeys);
                fileContent.ToList().ForEach(item => this.ListAllKeyBind.Items.Add(new KeyBind(item)));
            }
            catch (Exception e)
            {
                new MessageBox("Проверьте состояние игры").Show();
                Console.WriteLine(e);
            }
        }

        private void ButtonStartKeyBoardSearch_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _keyboardCheck.Run();
        }

        private void ButtonStartConsoleCommandSearch_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _consoleCommand.Run();
        }
    }
}