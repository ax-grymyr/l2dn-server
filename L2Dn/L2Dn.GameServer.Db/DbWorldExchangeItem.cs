namespace L2Dn.GameServer.Db;

public class DbWorldExchangeItem
{
    public int Id { get; set; }
    public int ItemObjectId { get; set; }
    public int ItemStatus { get; set; } // TODO enum WorldExchangeItemStatusType
    public int CategoryId { get; set; } // TODO enum WorldExchangeItemSubType
    public long Price { get; set; }
    public int OldOwnerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}