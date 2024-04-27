using L2Dn.AuthServer.Db;
using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace L2Dn.AuthServer;

public class DbFactory: PooledDbContextFactory<AuthServerDbContext>
{
    private static DbFactory? _instance;

    private DbFactory(DbContextOptions<AuthServerDbContext> options, int poolSize)
        : base(options, poolSize)
    {
    }

    public static DbFactory Instance =>
        _instance ?? throw new InvalidOperationException("Database factory is not initialized");

    public static void Initialize(DatabaseConfig config)
    {
        _instance = new DbFactory(DesignTimeAuthServerDbContextFactory.GetOptions(config), config.PoolSize);
    }

    public static void UpdateDatabase(DatabaseConfig config)
    {
        using AuthServerDbContext ctx = new(DesignTimeAuthServerDbContextFactory.GetOptions(config));
        ctx.Database.Migrate();
    }
}