using L2Dn.Configuration;
using Microsoft.EntityFrameworkCore;
using NLog;
using Npgsql;

namespace L2Dn.GameServer.Db;

public class GameServerDbContext: DbContext
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(GameServerDbContext));
    public static DatabaseConfig? Config { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DatabaseConfig? databaseConfig = Config;
            if (databaseConfig is null)
            {
                ConfigBase config = ConfigurationUtil.LoadConfig<ConfigBase>();
                databaseConfig = config.Database;
            }

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
                    data => { _logger.Log(LogLevel.FromOrdinal((int)data.LogLevel), data.ToString()); });
            }
        }        
    }

    public DbSet<AccountRef> AccountRefs => Set<AccountRef>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterFriend> CharacterFriends => Set<CharacterFriend>();
    public DbSet<CharacterSummon> CharacterSummons => Set<CharacterSummon>();
    public DbSet<CharacterOfflineTrade> CharacterOfflineTrades => Set<CharacterOfflineTrade>();
    public DbSet<CharacterOfflineTradeItem> CharacterOfflineTradeItems => Set<CharacterOfflineTradeItem>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<DbItem> Items => Set<DbItem>();
     public DbSet<Clan> Clans => Set<Clan>();
    public DbSet<ClanPrivileges> ClanPrivileges => Set<ClanPrivileges>();
    public DbSet<ClanSkill> ClanSkills => Set<ClanSkill>();
    public DbSet<ClanSubPledge> ClanSubPledges => Set<ClanSubPledge>();
    public DbSet<ClanNotice> ClanNotices => Set<ClanNotice>();
    public DbSet<ClanWar> ClanWars => Set<ClanWar>();
    public DbSet<Ally> Allys => Set<Ally>();
    public DbSet<Crest> Crests => Set<Crest>();
    
    public DbSet<Forum> Forums => Set<Forum>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Post> Posts => Set<Post>();
    
    public DbSet<Announcement> Announcements => Set<Announcement>();

    public DbSet<BotReport> BotReports => Set<BotReport>();
    public DbSet<BufferScheme> BufferSchemes => Set<BufferScheme>();
    public DbSet<BuyList> BuyLists => Set<BuyList>();
}