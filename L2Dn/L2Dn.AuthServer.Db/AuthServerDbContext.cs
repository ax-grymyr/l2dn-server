using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Db;

public class AuthServerDbContext(DbContextOptions options)
    : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<GameServer> GameServers => Set<GameServer>();
}