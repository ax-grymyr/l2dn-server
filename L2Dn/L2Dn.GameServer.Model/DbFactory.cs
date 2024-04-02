using L2Dn.Configuration;
using L2Dn.GameServer.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace L2Dn.GameServer;

public class DbFactory: PooledDbContextFactory<GameServerDbContext>
{
    private static DbFactory? _instance;

    private DbFactory(DbContextOptions<GameServerDbContext> options, int poolSize)
        : base(options, poolSize)
    {
    }

    public static DbFactory Instance =>
        _instance ?? throw new InvalidOperationException("Database factory is not initialized");

    public static void Initialize(DatabaseConfig config)
    {
        _instance = new DbFactory(DesignTimeGameServerDbContextFactory.GetOptions(config), config.PoolSize);
    } 
}