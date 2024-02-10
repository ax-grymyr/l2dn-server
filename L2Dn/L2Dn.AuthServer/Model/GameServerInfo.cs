using System.Net;
using L2Dn.DbModel;

namespace L2Dn.AuthServer.Model;

public sealed class GameServerInfo
{
    public int ServerId { get; set; }
    
    public IPAddress Address { get; set; } = IPAddress.Loopback;
    public int Port { get; set; } = 7777;

    public int PlayerCount { get; set; }
    public int MaxPlayerCount { get; set; }

    public bool IsPvpServer { get; set; }
    public GameServerAttributes Attributes { get; set; }
    public bool IsOnline { get; set; }
    
    public bool Brackets { get; set; }
}
