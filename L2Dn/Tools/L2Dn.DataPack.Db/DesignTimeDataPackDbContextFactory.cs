using System.Data.Common;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using NLog;

namespace L2Dn.DataPack.Db;

public sealed class DesignTimeDataPackDbContextFactory: IDesignTimeDbContextFactory<DataPackDbContext>
{
    public DataPackDbContext CreateDbContext(string[] args) => CreateDbContext("datapack.sqlite");

    public static DataPackDbContext CreateDbContext(string databasePath)
    {
        DataPackDbContext ctx = new(GetOptions(databasePath));
        return ctx;
    }

    public static DbContextOptions<DataPackDbContext> GetOptions(string databasePath)
    {
        DbContextOptionsBuilder<DataPackDbContext> optionsBuilder = new();
        ConfigureOptions(optionsBuilder, databasePath);
        return optionsBuilder.Options;
    }

    public static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder, string databasePath = "datapack.sqlite")
    {
        SqliteConnectionStringBuilder sb = new()
        { 
            DataSource = databasePath,
            ForeignKeys = true,
        };
            
        optionsBuilder.UseSqlite(sb.ToString());

        Microsoft.Extensions.Logging.LogLevel logLevel = (Microsoft.Extensions.Logging.LogLevel)LogLevel.Warn.Ordinal;
        Logger logger = LogManager.GetLogger("Database");
        optionsBuilder.LogTo((_, level) => level >= logLevel,
                data => { logger.Log(LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); })
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();

        optionsBuilder.AddInterceptors(new CreateCollationsInterceptor());
        
        optionsBuilder.EnableThreadSafetyChecks(false);
    }

    public static void ConfigureModel(ModelBuilder modelBuilder)
    {
        // Temporary hack to set collation
        foreach (var collation in GetPropertiesWithAttribute<CollationAttribute>(modelBuilder))
            collation.Property.SetCollation(collation.Attribute.Collation);

        // Temporary hack to add foreign keys
        foreach (var reference in GetPropertiesWithAttribute<ReferenceAttribute>(modelBuilder).ToList())
        {
            modelBuilder.Entity(reference.EntityType.ClrType,
                entityBuilder => entityBuilder.HasOne(reference.Attribute.ReferencedType).WithMany()
                    .HasForeignKey(reference.Property.Name));
        }
    }

    private static IEnumerable<EntityPropertyAttributeTriple<T>> GetPropertiesWithAttribute<T>(
        ModelBuilder modelBuilder)
        where T: Attribute
        => from entityType in modelBuilder.Model.GetEntityTypes()
            from property in entityType.GetProperties()
            let attribute = property.PropertyInfo?.GetCustomAttribute<T>()
            where attribute != null
            select new EntityPropertyAttributeTriple<T>(entityType, property, attribute);

    private readonly struct EntityPropertyAttributeTriple<T>(
        IMutableEntityType entityType,
        IMutableProperty property,
        T attribute)
    {
        public IMutableEntityType EntityType { get; } = entityType;
        public IMutableProperty Property { get; } = property;
        public T Attribute { get; } = attribute;
    }  
    
    private sealed class CreateCollationsInterceptor: IDbConnectionInterceptor
    {
        public DbConnection ConnectionCreated(ConnectionCreatedEventData eventData, DbConnection result)
        {
            SqliteConnection connection = (SqliteConnection)result;
        
            connection.CreateCollation(DataPackDbContext.CaseInsensitiveCollation,
                (a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a, b));

            return result;
        }
    }
}

public class ReferenceAttribute(Type referencedType): Attribute
{
    public Type ReferencedType { get; } = referencedType;
}

public class CollationAttribute(string collation): Attribute
{
    public string Collation { get; } = collation;
}
