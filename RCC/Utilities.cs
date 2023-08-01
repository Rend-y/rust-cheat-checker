using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;

namespace RCC
{
    public static class Utilities
    {
        public static void OpenDiscordServer() => Process.Start(Global.DiscordInviteLink);
        public static string PathToLocalApplication => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        public static IEnumerable<XElement> GetXmlDocumentFromWebProcess(string path_to_exe, string url, string path_to_save_xml)
        {
            new WebClient().DownloadFile(url, path_to_exe);
            Process.Start(path_to_exe, $"/sxml {path_to_save_xml}")?.WaitForExit();

            Thread deleteThread = new Thread(remove_file_list =>
            {
                if (remove_file_list is string[] fileList)
                {
                    fileList.ToList().ForEach(file =>
                    {
                        Thread.Sleep(500);
                        File.Delete(file);
                    });
                }
            });
            deleteThread.Start(new[] { path_to_exe, path_to_save_xml });

            return XDocument.Load(path_to_save_xml).Descendants("item");
        }
        public static void CheckOnUpdate()
        {
            string getFileVersionFromGit = new WebClient().DownloadString("https://raw.githubusercontent.com/Midoruya/rust-cheat-checker/main/version.ini");
            string getFileVersionFromAssembly = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
            if (getFileVersionFromGit.Equals(getFileVersionFromAssembly) == false)
            {
                DialogResult buttonPressed = MessageBox.Show(
                    "Вышла новая версия вы желаете обновится ?",
                    "Обновление",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                if (buttonPressed == DialogResult.Yes)
                {
                    new WebClient().DownloadFile($"https://github.com/Midoruya/rust-cheat-checker/releases/download/{getFileVersionFromGit}/RCC.exe", "Updated.exe");
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C timeout 5 & del RCC.exe & move Updated.exe RCC.exe & del Updated.exe & runas RCC.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };
                    Process.Start(startInfo);
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }
        public static bool IsAdminStartup()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}