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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new ReferenceConvention());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbIcon>(entityBuilder =>
        {
            entityBuilder.Property(x => x.Name).UseCollation(CaseInsensitiveCollation);
        });

        // modelBuilder.Entity<DbItemCreateList>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbItem>().WithMany().HasForeignKey(x => x.BoxItemId);
        // });

        // modelBuilder.Entity<DbItemCreateListItem>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbItem>().WithMany().HasForeignKey(x => x.ItemId);
        //     entityBuilder.HasOne<DbItemCreateList>().WithMany().HasForeignKey(x => x.ListId);
        // });
        //
        // modelBuilder.Entity<DbItemRelatedQuest>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbItem>().WithMany().HasForeignKey(x => x.ItemId);
        //     // TODO: quest foreign key
        // });

        // modelBuilder.Entity<DbBuyListNpc>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbBuyList>().WithMany().HasForeignKey(x => x.BuyListId);
        // });
        //
        // modelBuilder.Entity<DbBuyListItem>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbBuyList>().WithMany().HasForeignKey(x => x.BuyListId);
        // });
 
        // modelBuilder.Entity<DbMultiSellListNpc>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbMultiSellList>().WithMany().HasForeignKey(x => x.MultiSellListId);
        // });
        //
        // modelBuilder.Entity<DbMultiSellListEntry>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbMultiSellList>().WithMany().HasForeignKey(x => x.MultiSellListId);
        // });
        //
        // modelBuilder.Entity<DbMultiSellListIngredient>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbMultiSellListEntry>().WithMany().HasForeignKey(x => x.MultiSellListEntryId);
        // });
        //
        // modelBuilder.Entity<DbMultiSellListProduct>(entityBuilder =>
        // {
        //     entityBuilder.HasOne<DbMultiSellListEntry>().WithMany().HasForeignKey(x => x.MultiSellListEntryId);
        // });
    }
}

public class ReferenceAttribute(Type referencedType): Attribute
{
    public Type ReferencedType { get; } = referencedType;
}

public class ReferenceConvention: IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
        {
            foreach (IConventionProperty property in entityType.GetProperties())
            {
                ReferenceAttribute? referenceAttribute =
                    property.PropertyInfo?.GetCustomAttribute<ReferenceAttribute>();
                
                if (referenceAttribute != null)
                {
                    IConventionEntityType? referencedEntityType =
                        modelBuilder.Metadata.FindEntityType(referenceAttribute.ReferencedType);

                    if (referencedEntityType == null)
                        throw new InvalidOperationException("Invalid reference type");

                    IConventionKey? referencedKey = referencedEntityType.FindPrimaryKey();
                    if (referencedKey == null)
                        throw new InvalidOperationException("Invalid reference type: no primary key");
                    
                    entityType.AddForeignKey(property, referencedKey, referencedEntityType);
                }
            }
        }
    }
}