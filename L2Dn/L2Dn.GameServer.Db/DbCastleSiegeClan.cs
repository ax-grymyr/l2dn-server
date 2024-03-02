using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(CastleId))]
public class DbCastleSiegeClan
{
    public byte CastleId { get; set; }
    public int ClanId { get; set; }
    public byte Type { get; set; }
    public bool IsCastleOwner { get; set; }
}