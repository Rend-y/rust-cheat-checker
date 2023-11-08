using System.Net;

namespace RCC.Modules.Server;

public interface IServerService
{
    public WebClient Client { get; }
    public void CheckOnUpdate();
}