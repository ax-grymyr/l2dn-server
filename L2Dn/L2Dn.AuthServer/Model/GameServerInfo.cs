using L2Dn.Network;
using L2Dn.Utilities;

namespace L2Dn.AuthServer.Model;

internal sealed class GameServerInfo
{
    public byte ServerId { get; set; }

    public int Address { get; set; } = IPAddressUtil.Loopback;
    public int Port { get; set; } = 7777;

    public int PlayerCount { get; set; }
    public int MaxPlayerCount { get; set; } = 5000;

    public byte AgeLimit { get; set; }
    public bool IsPvpServer { get; set; }
    public GameServerAttributes Attributes { get; set; }
    public bool IsOnline { get; set; }

    public bool Brackets { get; set; }

    public string? AccessKey { get; set; }
    public bool FromDatabase { get; set; }

    public Connection? Connection { get; set; }
}