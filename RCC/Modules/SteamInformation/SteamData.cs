using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RCC.Modules.SteamInformation
{
    public class SteamData
    {
        private readonly BitmapSource _avatar;

        public SteamData(in long steamId, in bool isDeleted = false)
        {
            SteamId = steamId;
            IsDeleted = isDeleted;
            var url = $"https://steamcommunity.com/profiles/{steamId}";

            try
            {
                using var client = new HttpClient();
                var taskGetHtml = client.GetStringAsync(url);
                taskGetHtml.Wait();
                var content = taskGetHtml.Result;


                IsHideAccount = Regex.Match(content, @"<div class=""profile_private_info"">([^"">]+)</div>")
                    .Groups[1].Length > 0; // parse is hide profile

                Username = Regex.Match(content, @"<span class=""actual_persona_name"">([^"">]+)</span>").Groups[1]
                    .Value; // parse username

                AccountLevel = IsHideAccount ? -1 :
                    int.TryParse(
                        Regex.Match(content, @"<span class=""friendPlayerLevelNum"">([^"">]+)</span>").Groups[1].Value,
                        out var level) ? level : -1;

                var avatarUrlArray = Regex.Matches(content, @"<img src=""([^"">]+)"">").Cast<Match>()
                    .Select(x => x.Groups[1].Value)
                    .FirstOrDefault(img => img.Contains("_full"));

                if (!string.IsNullOrEmpty(avatarUrlArray))
                {
                    AvatarUrl = avatarUrlArray;
                    GetSteamAccountAvatar(avatarUrlArray, out _avatar);
                }
            }
            catch (Exception exception)
            {
                Debug.Fail(exception.Message, exception.StackTrace);
            }
        }

        public SteamData(in string username, in long steamId, in int accountLevel, in string avatarUrl,
            in bool isHideAccount, in bool isDeleted)
        {
            Username = username;
            SteamId = steamId;
            AccountLevel = accountLevel;
            IsHideAccount = isHideAccount;
            IsDeleted = isDeleted;
            string correctAvatarUrl = avatarUrl == string.Empty
                ? "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg"
                : avatarUrl;
            AvatarUrl = correctAvatarUrl;
            GetSteamAccountAvatar(correctAvatarUrl, out _avatar);
        }

        public string Username { get; }
        public long SteamId { get; }
        public int AccountLevel { get; }
        public string AvatarUrl { get; private set; }
        public bool IsHideAccount { get; }
        public bool IsDeleted { get; }
        public ImageSource GetAccountAvatar => _avatar;
        public string GetAccountLevel => AccountLevel.ToString();
        public string GetUsername => $"Username : {Username}";
        public string GetSteamId => $"Steam Id : {SteamId}";
        public Visibility GetIsHideForWindow => IsHideAccount ? Visibility.Visible : Visibility.Hidden;
        public Visibility GetIsDeleted => IsDeleted ? Visibility.Visible : Visibility.Hidden;

        private void GetSteamAccountAvatar(in string avatarUrl, out BitmapSource avatar)
        {
            using (WebClient client = new WebClient())
            {
                using (Stream stream = client.OpenRead(avatarUrl)!)
                {
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        public static bool IsSteamId(in long steamId) => steamId.ToString().Length == 17;
    }
}