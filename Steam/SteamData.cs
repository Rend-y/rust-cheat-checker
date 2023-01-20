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
        public readonly string username;
        public readonly long steam_id;
        public readonly int account_level;
        private readonly BitmapSource avatar;
        public readonly string avatar_url;
        public bool is_hide_account;
        public steam_data() { }
        public steam_data(string username, long stem_id, int account_level, string avatar_url, bool is_hide_account)
        {
            this.username = username;
            this.steam_id = stem_id;
            this.account_level = account_level;
            WebClient client = new WebClient();
            if (avatar_url == string.Empty)
                avatar_url = "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg";
            Stream stream = client.OpenRead(avatar_url);
            if (stream != null)
            {
                Bitmap bitmap = new Bitmap(stream);
                stream.Close();
                this.avatar_url = avatar_url;
                this.avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            this.is_hide_account = is_hide_account;
        }

        public ImageSource get_account_avatar => this.avatar;
        public string get_account_level => this.account_level.ToString();
        public string get_username => $"Username : {this.username}";
        public string get_steam_id => $"Steam Id : {this.steam_id}";
        public Visibility get_is_hide_for_window => this.is_hide_account ? Visibility.Visible : Visibility.Hidden;
    }
}
