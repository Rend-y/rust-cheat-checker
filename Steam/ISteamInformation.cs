using System.Collections.Generic;

namespace RCC.Steam;

public interface ISteamInformation<out TSteamData>
{
    /// <summary>
    /// Use this function for get path to folder (steam)
    /// </summary>
    /// <returns>full path to steam</returns>
    string GetSteamLocation();
    /// <summary>
    /// use this for get full path to file which keeps all accounts data
    /// </summary>
    /// <returns>full path to file which keeps all accounts data</returns>
    List<string> GetSteamIdFromContent(string content);

    List<string> GetSteamIdFromCoPlayData();
    /// <summary>
    /// Use this to get steam data (avatar, username, is hide account, account level)
    /// if user account is hide. Then we return the level -1
    /// </summary>
    /// <param name="steamId">steam id in steam</param>
    /// <param name="isDeleted">steam account has been cleaned</param>
    /// <returns>steam data (avatar, username, is hide account, account level)</returns>
    public TSteamData GetSteamData(in long steamId, in bool isDeleted = default);
    /// <summary>
    /// use this to get last account
    /// </summary>
    /// <returns>Steam Data for current user (avatar, username, is hide account, account level)</returns>
    public TSteamData GetLastSteamAccountInfo();
    /// <summary>
    /// use this for get full path to file which keeps all accounts data
    /// </summary>
    /// <returns>full path to file which keeps all accounts data</returns>
    public string PathToLoginData { get; }
    public string PathToSteamConfig { get; }
}