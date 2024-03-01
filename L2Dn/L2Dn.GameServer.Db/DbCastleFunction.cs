using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CastleId), nameof(Type))]
public class DbCastleFunction
{
    public byte CastleId { get; set; }
    public byte Type { get; set; }
    public short Level { get; set; }
    public int Lease { get; set; }
    public TimeSpan Rate { get; set; }
    public DateTime? EndTime { get; set; }
}