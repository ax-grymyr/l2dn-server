using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(CastleId))]
public class DbSiegeClan
{
    public byte CastleId { get; set; }
    public int ClanId { get; set; }
    public byte Type { get; set; }
    public bool CastleOwmer { get; set; }
}