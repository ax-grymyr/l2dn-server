using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuildDataPackDb.Db;

public class DbBuyList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int BuyListId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;
}

[PrimaryKey(nameof(BuyListId), nameof(NpcId))]
public class DbBuyListNpc
{
    public int BuyListId { get; set; }
    public int NpcId { get; set; }
}

[PrimaryKey(nameof(BuyListId), nameof(ItemId))]
public class DbBuyListItem
{
    public int BuyListId { get; set; }
    public int ItemId { get; set; }
    public long Price { get; set; }
    public long? Count { get; set; }
    public TimeSpan? RestockDelay { get; set; }

    public bool Enabled { get; set; } = true;
}