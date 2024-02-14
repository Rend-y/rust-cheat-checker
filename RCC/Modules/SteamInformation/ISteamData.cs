using System.Windows;
using System.Windows.Media;

namespace RCC.Modules.SteamInformation;

public interface ISteamData
{
    string Username { get; }
    long SteamId { get; }
    int AccountLevel { get; }
    string AvatarUrl { get; }
    bool IsHideAccount { get; }
    bool IsDeleted { get; }
    ImageSource GetAccountAvatar { get; }
    string GetAccountLevel { get; }
    string GetUsername { get; }
    string GetSteamId { get; }
    Visibility GetIsHideForWindow { get; }
    Visibility GetIsDeleted { get; }
}