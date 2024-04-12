using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BuildDataPackDb.Db;

public class DbMultiSellList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int MultiSellListId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;
    public bool ApplyTaxes { get; set; }
    public bool IsChanceMultiSell { get; set; }
    public bool MaintainEnchantment { get; set; }
    public double IngredientMultiplier { get; set; } = 1.0;
    public double ProductMultiplier { get; set; } = 1.0;
}

[PrimaryKey(nameof(MultiSellListId), nameof(NpcId))]
public class DbMultiSellListNpc
{
    public int MultiSellListId { get; set; }
    public int NpcId { get; set; }
}

[Index(nameof(MultiSellListId), nameof(Order), IsUnique = true)]
public class DbMultiSellListEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MultiSellListEntryId { get; set; }

    public int MultiSellListId { get; set; }
    public int Order { get; set; }
    public bool Enabled { get; set; } = true;
}

[PrimaryKey(nameof(MultiSellListEntryId), nameof(Order))]
public class DbMultiSellListIngredient
{
    public int MultiSellListEntryId { get; set; }
    public int Order { get; set; }
    public int ItemId { get; set; }
    public long Count { get; set; } = 1;
    public byte EnchantLevel { get; set; }
    public bool MaintainIngredient { get; set; }
}

[PrimaryKey(nameof(MultiSellListEntryId), nameof(Order))]
public class DbMultiSellListProduct
{
    public int MultiSellListEntryId { get; set; }
    public int Order { get; set; }
    public int ItemId { get; set; }
    public long Count { get; set; } = 1;
    public byte EnchantLevel { get; set; }
    public double Chance { get; set; } = 100.0;
}