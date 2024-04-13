using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
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