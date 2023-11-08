using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Xml.Linq;

namespace RCC
{
    public static class Utilities
    {
        public static string PathToLocalApplication => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ??
                                                       throw new InvalidOperationException(
                                                           "Path to local application is null");

        public static void OpenDiscordServer() => Process.Start(Global.DiscordInviteLink);

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return value.ToString();
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static IEnumerable<XElement> GetXmlDocumentFromWebProcess(string pathToExe, string url,
            string pathToSaveXml)
        {
            new WebClient().DownloadFile(url, pathToExe);
            Process.Start(pathToExe, $"/sxml {pathToSaveXml}")?.WaitForExit();

            var deleteThread = new Thread(removeFileList =>
            {
                if (removeFileList is string[] fileList)
                {
                    fileList.ToList().ForEach(file =>
                    {
                        Thread.Sleep(500);
                        File.Delete(file);
                    });
                }
            });
            deleteThread.Start(new[] { pathToExe, pathToSaveXml });

            return XDocument.Load(pathToSaveXml).Descendants("item");
        }

        public static bool IsAdminStartup()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}