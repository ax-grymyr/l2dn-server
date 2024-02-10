using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace L2Dn.GameServer.Db;

public class GameServerDbContext: DbContext
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
    
    public DbSet<AccountRef> AccountRefs { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Clan> Clans { get; set; }
    public DbSet<Ally> Allys { get; set; }
    public DbSet<Crest> Crests { get; set; }
}