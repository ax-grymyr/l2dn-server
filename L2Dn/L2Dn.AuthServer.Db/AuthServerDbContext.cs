using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using NLog;
using Npgsql;

namespace L2Dn.AuthServer.Db;

public class AuthServerDbContext: DbContext
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AuthServerDbContext));
    public static string? ConnectionString { get; set; }
    public static bool Trace { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (ConnectionString is null)
            {
                ConfigBase config = ConfigurationUtil.LoadConfig<ConfigBase>();
                DatabaseConfig databaseConfig = config.Database;

                NpgsqlConnectionStringBuilder sb = new()
                {
                    Host = databaseConfig.Server,
                    Database = databaseConfig.DatabaseName,
                    Username = databaseConfig.UserName,
                    Password = databaseConfig.Password
                };

                ConnectionString = sb.ToString();
                Trace = databaseConfig.Trace;
            }

            optionsBuilder.UseNpgsql(ConnectionString);

            if (Trace)
            {
                optionsBuilder.LogTo((_, _) => true,
                    data => { _logger.Log(LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); });
            }
        }        
    }
    
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<GameServer> GameServers => Set<GameServer>();
}