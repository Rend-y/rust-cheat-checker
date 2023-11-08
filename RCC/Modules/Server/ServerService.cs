using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RCC.Modules.Server;

public class ServerService : IServerService
{
    public WebClient Client { get; } = new();

    public void CheckOnUpdate()
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version.ToString() ??
                      throw new InvalidOperationException(
                          "Get file version from assembly is null");
        var versionArray = version.Split('.');
        var serverVersion = string.Join(".", versionArray.Take(versionArray.Length - 1));
        var linkToVersion = $"https://github.com/Midoruya/rust-cheat-checker/releases/tag/{serverVersion}";
        var isLastVersion = false;
        try
        {
            var resultByte = Client.DownloadData(linkToVersion);
            var getHtml = Encoding.UTF8.GetString(resultByte);
            isLastVersion =
                getHtml.Contains(
                    @"<span data-view-component=""true"" class=""Label Label--success Label--large"">Latest</span>");
        }
        catch (Exception e)
        {
            // ignored
        }

        if (isLastVersion) return;
        var buttonPressed = MessageBox.Show(
            "Вышла новая версия вы желаете обновится ?",
            "Обновление",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly);
        if (buttonPressed == DialogResult.Yes)
        {
            Process.Start(linkToVersion);
            Environment.Exit(Environment.ExitCode);
        }
    }
}