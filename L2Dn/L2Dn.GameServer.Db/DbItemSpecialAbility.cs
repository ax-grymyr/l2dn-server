using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ItemId), nameof(OptionId))]
public class DbItemSpecialAbility
{
    public int ItemId { get; set; }
    public byte Type { get; set; }
    public int OptionId { get; set; }
    public byte Position { get; set; } 
}