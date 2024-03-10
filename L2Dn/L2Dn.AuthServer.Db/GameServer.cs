using System.ComponentModel.DataAnnotations;
using L2Dn.Model;

namespace L2Dn.AuthServer.Db;

public class GameServer
{
    [Key]
    public byte ServerId { get; set; }

    [MaxLength(50)]
    public string IPAddress { get; set; } = "127.0.0.1";

    public int Port { get; set; } = 7777;

    // Parameters
    public byte AgeLimit { get; set; } = 15;
    public bool IsPvpServer { get; set; }
    public GameServerAttributes Attributes { get; set; }
    public bool Brackets { get; set; }
    
    [MaxLength(50)]
    public string? AccessKey { get; set; }

    public short MaxPlayerCount { get; set; } = 5000;
}