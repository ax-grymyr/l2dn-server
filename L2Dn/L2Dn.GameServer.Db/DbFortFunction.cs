using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(FortId), nameof(Type))]
public class DbFortFunction
{
    public byte FortId { get; set; }
    public byte Type { get; set; }
    public short Level { get; set; }
    public int Lease { get; set; }
    public TimeSpan Rate { get; set; }
    public DateTime? EndTime { get; set; }
}