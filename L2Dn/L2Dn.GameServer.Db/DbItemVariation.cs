using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbItemVariation
{
    [Key]
    public int ItemId { get; set; }
    public int MineralId { get; set; }
    public int Option1 { get; set; }
    public int Option2 { get; set; }
}