using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(Date))]
public class DbCharacterRankingHistory
{
    public int CharacterId { get; set; }
    public DateOnly Date { get; set; }
    public int Ranking { get; set; }
    public long Exp { get; set; }
}