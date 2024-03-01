using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(ClanHallId), nameof(ClanId))]
public class DbClanHallBidder
{
    public int ClanHallId { get; set; }
    public int ClanId { get; set; }
    public long Bid { get; set; }
    public DateTime BidTime { get; set; }
}