using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CastleId), nameof(SeedId), nameof(NextPeriod))]
public class CastleManorProduction
{
    public short CastleId { get; set; }
    public int SeedId { get; set; }
    public long Amount { get; set; }
    public long StartAmount { get; set; }
    public long Price { get; set; }
    public bool NextPeriod { get; set; } = true;
}