using BuildDataPackDb.Db;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NLog;

namespace BuildDataPackDb;

public sealed class DesignTimeDataPackDbContextFactory: IDesignTimeDbContextFactory<DataPackDbContext>
{
    public DataPackDbContext CreateDbContext(string[] args) => CreateDbContext("datapack.sqlite");

    public static DataPackDbContext CreateDbContext(string databasePath)
    {
        DataPackDbContext ctx = new(GetOptions(databasePath));
        SqliteConnection connection = (SqliteConnection)ctx.Database.GetDbConnection();
        
        connection.CreateCollation(DataPackDbContext.CaseInsensitiveCollation,
            (a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a, b));

        return ctx;
    }

    public static DbContextOptions<DataPackDbContext> GetOptions(string databasePath)
    {
        SqliteConnectionStringBuilder sb = new()
        { 
            DataSource = databasePath,
            ForeignKeys = true,
        };
            
        DbContextOptionsBuilder<DataPackDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite(sb.ToString());

        Microsoft.Extensions.Logging.LogLevel logLevel = (Microsoft.Extensions.Logging.LogLevel)LogLevel.Warn.Ordinal;
        Logger logger = LogManager.GetLogger("Database");
        optionsBuilder.LogTo((_, level) => level >= logLevel,
                data => { logger.Log(LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); })
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
        
        optionsBuilder.EnableThreadSafetyChecks(false);
        
        return optionsBuilder.Options;
    }
}