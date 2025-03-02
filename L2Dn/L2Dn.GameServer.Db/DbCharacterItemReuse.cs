using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(ItemId), nameof(ItemObjectId))]
public class DbCharacterItemReuse
{
    public int CharacterId { get; set; }
    public int ItemId { get; set; }
    public int ItemObjectId { get; set; }
    public TimeSpan ReuseDelay { get; set; }
    public DateTime SysTime { get; set; }
}