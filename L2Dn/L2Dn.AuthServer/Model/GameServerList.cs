using System.Net;
using L2Dn.DbModel;
using L2Dn.Logging;

namespace L2Dn.AuthServer.Model;

public sealed class GameServerList: List<GameServerInfo>
{
    public static GameServerList Instance { get; } = new();
    
    public void UpdateFrom(List<GameServer> servers)
    {
        lock (this)
        {
            Dictionary<int, GameServerInfo> dictionary = this.ToDictionary(s => s.ServerId);
            foreach (GameServer server in servers)
            {
                if (dictionary.TryGetValue(server.ServerId, out GameServerInfo? info))
                {
                    dictionary.Remove(server.ServerId);
                    FillServerInfo(info, server);
                }
                else
                {
                    info = new GameServerInfo
                    {
                        ServerId = server.ServerId
                    };
                    
                    FillServerInfo(info, server);
                    int pos = servers.FindLastIndex(s => s.ServerId < server.ServerId);
                    Insert(pos + 1, info);
                }
            }
        }
    }

    private void FillServerInfo(GameServerInfo info, GameServer server)
    {
        if (!IPAddress.TryParse(server.Address, out IPAddress? address))
        {
            Logger.Error($"Error parsing IP address '{server.Address}'");
            address = IPAddress.Loopback;
        }
        
        info.Address = address;
        info.Port = server.Port;

        info.PlayerCount = server.PlayerCount;
        info.MaxPlayerCount = server.MaxPlayerCount;

        info.IsPvpServer = server.IsPvpServer;
        info.Attributes = server.Attributes;
        info.Brackets = server.Brackets;
        info.IsOnline = server.IsOnline;
    }
}
