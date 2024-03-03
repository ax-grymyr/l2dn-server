using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(CharacterId), nameof(GroupId))]
public class DbEnchantChallengePoint
{
    public int CharacterId { get; set; }
    public int GroupId { get; set; }
    public int Points { get; set; }
}