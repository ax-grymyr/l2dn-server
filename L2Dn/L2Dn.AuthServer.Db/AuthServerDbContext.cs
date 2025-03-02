using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Db;

public class AuthServerDbContext(DbContextOptions options)
    : DbContext(options)
{
    public DbSet<DbAccount> Accounts => Set<DbAccount>();
    public DbSet<DbGameServer> GameServers => Set<DbGameServer>();
    public DbSet<DbAccountCharacterData> AccountCharacterData => Set<DbAccountCharacterData>();
}