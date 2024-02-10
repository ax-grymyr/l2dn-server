using L2Dn.AuthServer.Db;

namespace L2Dn.AuthServer.Model;

public sealed class GameServerInfo
{
    public byte ServerId { get; set; }

    public int Address { get; set; } = 0x0100007F; // 127.0.0.1
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
}