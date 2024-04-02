using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NLog;
using Npgsql;

namespace L2Dn.GameServer.Db;

public sealed class DesignTimeGameServerDbContextFactory: IDesignTimeDbContextFactory<GameServerDbContext>
{
    public GameServerDbContext CreateDbContext(string[] args)
    {
        ConfigBase config = ConfigurationUtil.LoadConfig<ConfigBase>();
        return new GameServerDbContext(GetOptions(config.Database));
    }

    public static DbContextOptions<GameServerDbContext> GetOptions(DatabaseConfig config)
    {
        NpgsqlConnectionStringBuilder sb = new()
        {
            Host = config.Server,
            Database = config.DatabaseName,
            Username = config.UserName,
            Password = config.Password
        };

        DbContextOptionsBuilder<GameServerDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(sb.ToString());

        if (config.Trace)
        {
            Logger logger = LogManager.GetLogger("Database");
            optionsBuilder.LogTo((_, _) => true,
                data => { logger.Log(LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); });
        }
        
        optionsBuilder.EnableThreadSafetyChecks(false);
        
        return optionsBuilder.Options;
    }
}