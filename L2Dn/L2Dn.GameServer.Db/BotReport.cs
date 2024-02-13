using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(BotId), nameof(ReporterId))]
public class BotReport
{
    public int BotId { get; set; }
    public int ReporterId { get; set; }
    public DateTime ReportTime { get; set; }
}