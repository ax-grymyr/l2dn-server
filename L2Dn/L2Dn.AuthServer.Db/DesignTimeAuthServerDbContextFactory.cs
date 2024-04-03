using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NLog;
using Npgsql;

namespace L2Dn.AuthServer.Db;

public sealed class DesignTimeAuthServerDbContextFactory: IDesignTimeDbContextFactory<AuthServerDbContext>
{
    public AuthServerDbContext CreateDbContext(string[] args)
    {
        ConfigBase config = ConfigurationUtil.LoadConfig<ConfigBase>();
        return new AuthServerDbContext(GetOptions(config.Database));
    }

    public static DbContextOptions<AuthServerDbContext> GetOptions(DatabaseConfig config)
    {
        NpgsqlConnectionStringBuilder sb = new()
        {
            Host = config.Server,
            Database = config.DatabaseName,
            Username = config.UserName,
            Password = config.Password
        };

        DbContextOptionsBuilder<AuthServerDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(sb.ToString());

        Microsoft.Extensions.Logging.LogLevel logLevel = (Microsoft.Extensions.Logging.LogLevel)config.LogLevel.Ordinal;
        Logger logger = LogManager.GetLogger("Database");
        optionsBuilder.LogTo((_, level) => level >= logLevel,
            data => { logger.Log(LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); }).EnableDetailedErrors();
        
        optionsBuilder.EnableThreadSafetyChecks(false);
        
        return optionsBuilder.Options;
    }
}