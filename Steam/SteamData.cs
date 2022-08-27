using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace RCC.Steam
{
    public class SteamData
    {
        public string username;
        public long steam_id;
        public int account_level;
        public BitmapSource avatar;
        public string avatar_url;
        public bool is_hide_account;

        public SteamData(string username, long stem_id, int accoung_level, string avatar_url, bool is_hide_account)
        {
            this.username = username;
            this.steam_id = stem_id;
            this.account_level = accoung_level;
            WebClient client = new WebClient();
            Bitmap bitmap;
            if (avatar_url == string.Empty)
                avatar_url = "https://avatars.cloudflare.steamstatic.com/a8c5d92192f114f5ed05a03a86e53facc7d22a27_full.jpg";
            Stream stream = client.OpenRead(avatar_url);
            bitmap = new Bitmap(stream);
            stream.Close();
            this.avatar_url = avatar_url;
            this.avatar = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); ;
            this.is_hide_account = is_hide_account;
        }

        public ImageSource get_account_avatar => this.avatar;
        public string get_account_level => this.account_level.ToString();
        public string get_username => $"Username : {this.username}";
        public string get_steam_id => $"Steam Id : {this.steam_id}";
        public Visibility get_is_hide_for_window => this.is_hide_account ? Visibility.Visible : Visibility.Hidden;
    }
}
