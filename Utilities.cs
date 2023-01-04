using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;

namespace RCC
{
    public abstract class Utilities
    {
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
    }
}