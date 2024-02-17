using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[Index(nameof(OwnerId), nameof(ItemId), nameof(Location))]
[Index(nameof(OwnerId), nameof(Location))]
[Index(nameof(ItemId))]
public class DbItem
{
    [Key]
    public int ObjectId { get; set; }

    public int OwnerId { get; set; }
    public int ItemId { get; set; }
    public long Count { get; set; }
    public int EnchantLevel { get; set; }
    public int Location { get; set; } // TODO: enum
    public int LocationData { get; set; }
    public int TimeOfUse { get; set; }
    public int CustomType1 { get; set; }
    public int CustomType2 { get; set; }
    public int ManaLeft { get; set; } = -1;
    public DateTime Time { get; set; }
}