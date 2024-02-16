using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CastleId), nameof(CropId), nameof(NextPeriod))]
public class CastleManorProcure
{
    public short CastleId { get; set; }
    public int CropId { get; set; }
    public long Amount { get; set; }
    public long StartAmount { get; set; }
    public long Price { get; set; }
    public int RewardType { get; set; }
    public bool NextPeriod { get; set; } = true;
}