using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace L2Dn.AuthServer.Db;

public class AuthServerDbContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            const string devConfigFile = "config.dev.json"; 
            if (File.Exists(devConfigFile))
            {
                BaseConfig config = ConfigurationUtil.LoadConfig<BaseConfig>();
                DatabaseConfig databaseConfig = config.Database;
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
                        data => { Logger.Log(NLog.LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); });
                }
            }
        }        
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<GameServer> GameServers { get; set; }
}