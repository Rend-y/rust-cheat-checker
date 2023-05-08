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
    public class steam_data
    {
        public string Username { get; }
        public readonly long SteamId;
        public readonly int AccountLevel;
        private readonly BitmapSource _avatar;
        public readonly string AvatarUrl;
        public readonly bool IsHideAccount;
        public bool IsDeleted;
        public steam_data() { }
        public steam_data(string username, long stem_id, int account_level, string avatar_url, bool is_hide_account, bool isDeleted = false)
        {
            this.Username = username;
            this.SteamId = stem_id;
            this.AccountLevel = account_level;
            WebClient client = new WebClient();
            if (avatar_url == string.Empty)
                avatar_url = "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg";
            Stream stream = client.OpenRead(avatar_url);
            if (stream != null)
            {
                Bitmap bitmap = new Bitmap(stream);
                stream.Close();
                this.AvatarUrl = avatar_url;
                this._avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            this.IsHideAccount = is_hide_account;
            this.IsDeleted = isDeleted;
        }

        public ImageSource GetAccountAvatar => this._avatar;
        public string GetAccountLevel => this.AccountLevel.ToString();
        public string GetUsername => $"Username : {this.Username}";
        public string GetSteamId => $"Steam Id : {this.SteamId}";
        public Visibility GetIsHideForWindow => this.IsHideAccount ? Visibility.Visible : Visibility.Hidden;
        public Visibility GetIsDeleted => this.IsHideAccount ? Visibility.Visible : Visibility.Hidden;
    }
}
