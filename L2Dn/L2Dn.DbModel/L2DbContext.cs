using L2Dn.DbModel.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace L2Dn.DbModel;

public class L2DbContext: DbContext
{
    public static DatabaseConfig Config { get; set; } = new()
    {
        Server = "db",
        DatabaseName = "l2dev",
        UserName = "l2dev_user",
        Password = "l2dev_user_pass"
    };

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthData>().HasKey(x => new { x.ServerId, x.AccountId });
    }

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

    public DbSet<GameServer> GameServers => Set<GameServer>();
    public DbSet<AuthData> AuthData => Set<AuthData>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();
}
