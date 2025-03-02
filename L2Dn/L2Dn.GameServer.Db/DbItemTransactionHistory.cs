using System.ComponentModel.DataAnnotations;

namespace L2Dn.GameServer.Db;

public class DbItemTransactionHistory
{
    [Key]
    public DateTime CreatedTime { get; set; }
    public int ItemId { get; set; }
    public int TransactionType { get; set; }
    public int EnchantLevel { get; set; }
    public long Price { get; set; }
    public long Count { get; set; }
}