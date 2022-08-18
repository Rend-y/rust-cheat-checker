using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC.Steam
{
    public class SteamData
    {
        public int steam_id_64 { get; set; }
        public int steam_id_32 { get; set; }
        public string steam_id { get; set; }
        public string steam_id3 { get; set; }
        public string nick_name { get; set; }

        SteamData(int steam_id_64, int steam_id_32, string steam_id, string steam_id3, string nick_name)
        {
            this.steam_id_64 = steam_id_64;
            this.steam_id_32 = steam_id_32;
            this.steam_id = steam_id;
            this.steam_id3 = steam_id3;
            this.nick_name = nick_name;
        }
    }
}
