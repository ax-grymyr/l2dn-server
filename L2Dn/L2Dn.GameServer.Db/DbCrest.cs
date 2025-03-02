using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbCrest
{
    public int Id { get; set; }

    [MaxLength(2176)]
    public byte[] Data { get; set; } = [];

    public byte Type { get; set; }
}