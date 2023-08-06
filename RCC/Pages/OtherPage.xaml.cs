using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using RCC.Modules.DangerousApp;
using RCC.QuickCheck;
using RCC.windows;

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

    public partial class OtherPage : Page
    {
        private readonly IDangerousApp<SDangerousApplication> _dangerousApps;

        public OtherPage(IDangerousApp<SDangerousApplication> dangerousApp,
            IDangerousApp<SDangerousApplication> dangerousApps)
        {
            _dangerousApps = dangerousApps;
            InitializeComponent();
            _dangerousApps.ListFindDangerousApplications.ForEach(item => ListAllDangerousApps.Items.Add(item));
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
            KeyboardCheck keyboardCheck = new KeyboardCheck();
        }

        private void ButtonStartConsoleCommandSearch_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConsoleCommand consoleCommand = new ConsoleCommand();
        }
    }
}