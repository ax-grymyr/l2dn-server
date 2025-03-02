using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(FortId), nameof(ClanId))]
public class DbFortSiegeClan
{
    public short FortId { get; set; }
    public int ClanId { get; set; }
}