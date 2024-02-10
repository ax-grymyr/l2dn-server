using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace L2Dn.AuthServer.Db;

public class AuthServerDbContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", true)
                .AddJsonFile("config.dev.json", true)
                .Build();

            string? connectionString = configuration.GetConnectionString("AuthServerDb");
            Console.WriteLine(configuration.GetConnectionString("AuthServerDb"));
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseNpgsql(connectionString);
            }

            string? loggingLevel = configuration.GetSection("Logging").GetSection("LogLevel")["Database"];
            if (loggingLevel == "Trace")
            {
                optionsBuilder.LogTo(Console.WriteLine);
            }
        }        
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<GameServer> GameServers { get; set; }
}