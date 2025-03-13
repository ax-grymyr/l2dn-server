using L2Dn.GameServer.Enums;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanId), nameof(CastleId))]
public class DbCastleSiegeClan
{
    public byte CastleId { get; set; }
    public int ClanId { get; set; }
    public SiegeParticipantType Type { get; set; }
    public bool IsCastleOwner { get; set; }
}