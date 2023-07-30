using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using MessageBox = RCC.windows.MessageBox;

namespace RCC.Steam
{
    public class SteamData
    {
        public string Username { get; private set; }
        public long SteamId { get; private set; }
        public  int AccountLevel { get; private set; }
        private readonly BitmapSource _avatar;
        public string AvatarUrl{ get; private set; }
        public bool IsHideAccount { get; private set; }
        public bool IsDeleted { get; private set; }

        public SteamData(in long steamId, in bool isDeleted = false)
        {
            SteamId = steamId;
            IsDeleted = isDeleted;
            string url = "https://steamcommunity.com/profiles/" + steamId;
            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] data = client.DownloadData(url); // download full markup a frontend
                    string text = Encoding.UTF8.GetString(data); // parse it's to utf 8 encode

                    IsHideAccount = Regex.Match(text, @"<div\s+class=""profile_private_info"">([^"">]+)</div>").Groups[1].Length > 0; // parse is hide profile
                    Username = Regex.Match(text, @"<span class=""actual_persona_name"">([^"">]+)</span>").Groups[1].Value; // parse username
                    if (IsHideAccount) AccountLevel = -1;
                    else AccountLevel = int.Parse(Regex.Match(text, @"<span class=""friendPlayerLevelNum"">([^"">]+)</span>").Groups[1].Value); // parse level
                    var avatarUrlArray = Regex.Matches(text, @"<img src=""([^"">]+)"">").Cast<Match>().Select(x => x.Groups[1].Value).ToList(); // parse avatar
                    foreach (string img in avatarUrlArray)
                    {
                        if (img.Contains("_full"))
                        {
                            AvatarUrl = img;
                            GetSteamAccountAvatar(img, out _avatar);
                            break;
                        }
                    }
                    
                }
            }
            catch (Exception exception) { new MessageBox(exception.Message).Show(); };
        }
        public SteamData(in string username,in long steamId,in int accountLevel,in string avatarUrl,in bool isHideAccount,in bool isDeleted)
        {
            Username = username;
            SteamId = steamId;
            AccountLevel = accountLevel;
            IsHideAccount = isHideAccount;
            IsDeleted = isDeleted;
            string correctAvatarUrl = avatarUrl == string.Empty ? "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg" : avatarUrl;
            AvatarUrl = correctAvatarUrl;
            GetSteamAccountAvatar(correctAvatarUrl, out _avatar);
        }

        public void GetSteamAccountAvatar(in string avatarUrl, out BitmapSource avatar)
        {
            using (WebClient client = new WebClient())
            {
                using (Stream stream = client.OpenRead(avatarUrl)!)
                {
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
        public ImageSource GetAccountAvatar => this._avatar;
        public string GetAccountLevel => this.AccountLevel.ToString();
        public string GetUsername => $"Username : {this.Username}";
        public string GetSteamId => $"Steam Id : {this.SteamId}";
        public Visibility GetIsHideForWindow => this.IsHideAccount ? Visibility.Visible : Visibility.Hidden;
        public Visibility GetIsDeleted => this.IsDeleted ? Visibility.Visible : Visibility.Hidden;
    }
}
