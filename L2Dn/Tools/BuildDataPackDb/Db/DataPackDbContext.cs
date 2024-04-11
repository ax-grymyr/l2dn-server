using Microsoft.EntityFrameworkCore;

namespace BuildDataPackDb.Db;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbIcon>(entityBuilder =>
        {
            entityBuilder.Property(x => x.Name).UseCollation(CaseInsensitiveCollation);
        });

        modelBuilder.Entity<DbItemCreateList>(entityBuilder =>
        {
            entityBuilder.HasOne<DbItem>().WithMany().HasForeignKey(x => x.BoxItemId);
        });

        modelBuilder.Entity<DbItemCreateListItem>(entityBuilder =>
        {
            entityBuilder.HasOne<DbItem>().WithMany().HasForeignKey(x => x.ItemId);
            entityBuilder.HasOne<DbItemCreateList>().WithMany().HasForeignKey(x => x.ListId);
        });

        modelBuilder.Entity<DbItemRelatedQuest>(entityBuilder =>
        {
            entityBuilder.HasOne<DbItem>().WithMany().HasForeignKey(x => x.ItemId);
            // TODO: quest foreign key
        });

        modelBuilder.Entity<DbBuyListNpc>(entityBuilder =>
        {
            entityBuilder.HasOne<DbBuyList>().WithMany().HasForeignKey(x => x.BuyListId);
        });

        modelBuilder.Entity<DbBuyListItem>(entityBuilder =>
        {
            entityBuilder.HasOne<DbBuyList>().WithMany().HasForeignKey(x => x.BuyListId);
        });
    }
}