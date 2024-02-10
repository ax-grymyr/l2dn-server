using System.ComponentModel.DataAnnotations;

namespace L2Dn.AuthServer.Db;

public class GameServer
{
    [Key]
    public byte ServerId { get; set; }

    public bool IsPvpServer { get; set; }
    
    public GameServerAttributes Attributes { get; set; }

    public bool Brackets { get; set; }
}