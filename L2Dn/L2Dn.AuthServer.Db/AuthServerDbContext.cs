using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using NLog;
using Npgsql;

namespace L2Dn.AuthServer.Db;

public class AuthServerDbContext: DbContext
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AuthServerDbContext));
    public static DatabaseConfig? Config { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DatabaseConfig? databaseConfig = Config;
            if (databaseConfig is null)
            {
                ConfigBase config = ConfigurationUtil.LoadConfig<ConfigBase>();
                databaseConfig = config.Database;
            }

            NpgsqlConnectionStringBuilder sb = new()
            {
                Host = databaseConfig.Server,
                Database = databaseConfig.DatabaseName,
                Username = databaseConfig.UserName,
                Password = databaseConfig.Password
            };

            optionsBuilder.UseNpgsql(sb.ToString());

            if (databaseConfig.Trace)
            {
                optionsBuilder.LogTo((_, _) => true,
                    data => { _logger.Log(NLog.LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); });
            }
        }        
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<GameServer> GameServers { get; set; }
}