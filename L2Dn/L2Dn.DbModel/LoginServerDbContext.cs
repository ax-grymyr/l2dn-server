using L2Dn.DbModel.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace L2Dn.DbModel;

public class LoginServerDbContext: DbContext
{
    public static DatabaseConfig Config { get; set; } = new()
    {
        Server = "db",
        DatabaseName = "l2dev",
        UserName = "l2dev_user",
        Password = "l2dev_user_pass"
    };

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DatabaseConfig config = Config;
        NpgsqlConnectionStringBuilder sb = new()
        {
            Host = config.Server,
            Database = config.DatabaseName,
            Username = config.UserName,
            Password = config.Password
        };

        optionsBuilder.UseNpgsql(sb.ConnectionString);

        //optionsBuilder.LogTo(Console.WriteLine);
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<GameServer> GameServers { get; set; }
}