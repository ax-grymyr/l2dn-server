using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class ItemAuction
{
    [Key]
    public int AuctionId { get; set; }
    public int InstanceId { get; set; }
    public int AuctionItemId { get; set; }
    public DateTime StartingTime { get; set; }
    public DateTime EndingTime { get; set; }
    public byte AuctionStateId { get; set; }

}