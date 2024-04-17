using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace L2Dn.DataPack.Db;

public class DataPackDbContext(DbContextOptions options): DbContext(options)
{
    public const string CaseInsensitiveCollation = "CASE_INSENSITIVE";
    public DbSet<DbIcon> Icons => Set<DbIcon>();
    public DbSet<DbItem> Items => Set<DbItem>();
    public DbSet<DbItemCreateList> ItemCreateLists => Set<DbItemCreateList>();
    public DbSet<DbItemCreateListItem> ItemCreateListItems => Set<DbItemCreateListItem>();
    public DbSet<DbItemRelatedQuest> ItemRelatedQuests => Set<DbItemRelatedQuest>();

    public DbSet<DbBuyList> BuyLists => Set<DbBuyList>();
    public DbSet<DbBuyListNpc> BuyListNpcs => Set<DbBuyListNpc>();
    public DbSet<DbBuyListItem> BuyListItems => Set<DbBuyListItem>();

    public DbSet<DbMultiSellList> MultiSellLists => Set<DbMultiSellList>();
    public DbSet<DbMultiSellListNpc> MultiSellListNpcs => Set<DbMultiSellListNpc>();
    public DbSet<DbMultiSellListEntry> MultiSellListEntries => Set<DbMultiSellListEntry>();
    public DbSet<DbMultiSellListIngredient> MultiSellListIngredients => Set<DbMultiSellListIngredient>();
    public DbSet<DbMultiSellListProduct> MultiSellListProducts => Set<DbMultiSellListProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => DesignTimeDataPackDbContextFactory.ConfigureModel(modelBuilder);
}
