using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbItemOnGround
{
    [Key]
    public int ObjectId { get; set; }

    public int ItemId { get; set; }
    public long Count { get; set; }
    public int EnchantLevel { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public DateTime? DropTime { get; set; }
    public bool Equipable { get; set; }
}