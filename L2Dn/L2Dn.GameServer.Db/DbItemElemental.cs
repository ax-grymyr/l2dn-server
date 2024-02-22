using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ItemId), nameof(Type))]
public class DbItemElemental
{
    public int ItemId { get; set; }
    public byte Type { get; set; }
    public int Value { get; set; }
}