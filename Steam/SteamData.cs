using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace RCC.Steam
{
    public class SteamData
    {
        public string Username { get; }
        public readonly long SteamId;
        public readonly int AccountLevel;
        private readonly BitmapSource _avatar;
        public readonly string AvatarUrl;
        public readonly bool IsHideAccount;
        public readonly bool IsDeleted;
        public SteamData(string username, long steamId, int accountLevel, string avatarUrl, bool isHideAccount, bool isDeleted)
        {
            this.Username = username;
            this.SteamId = steamId;
            this.AccountLevel = accountLevel;
            WebClient client = new WebClient();
            if (avatarUrl == string.Empty)
                avatarUrl = "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg";
            Stream stream = client.OpenRead(avatarUrl);
            if (stream != null)
            {
                Bitmap bitmap = new Bitmap(stream);
                stream.Close();
                this.AvatarUrl = avatarUrl;
                this._avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            this.IsHideAccount = isHideAccount;
            this.IsDeleted = isDeleted;
        }

        public ImageSource GetAccountAvatar => this._avatar;
        public string GetAccountLevel => this.AccountLevel.ToString();
        public string GetUsername => $"Username : {this.Username}";
        public string GetSteamId => $"Steam Id : {this.SteamId}";
        public Visibility GetIsHideForWindow => this.IsHideAccount ? Visibility.Visible : Visibility.Hidden;
        public Visibility GetIsDeleted => this.IsDeleted ? Visibility.Visible : Visibility.Hidden;
    }
}
