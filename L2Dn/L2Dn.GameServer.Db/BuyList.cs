using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(BuyListId), nameof(ItemId))]
public class BuyList
{
    public int BuyListId { get; set; }
    public int ItemId { get; set; }
    public long Count { get; set; }
    public DateTime NextRestockTime { get; set; }
}