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
    public DbSet<AccountPremium> AccountPremiums => Set<AccountPremium>();
    public DbSet<AccountVariable> AccountVariables => Set<AccountVariable>();

    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterFriend> CharacterFriends => Set<CharacterFriend>();
    public DbSet<CharacterSummon> CharacterSummons => Set<CharacterSummon>();
    public DbSet<CharacterOfflineTrade> CharacterOfflineTrades => Set<CharacterOfflineTrade>();
    public DbSet<CharacterOfflineTradeItem> CharacterOfflineTradeItems => Set<CharacterOfflineTradeItem>();
    public DbSet<CharacterDailyReward> CharacterDailyRewards => Set<CharacterDailyReward>();
    public DbSet<CharacterSubClass> CharacterSubClasses => Set<CharacterSubClass>();
    public DbSet<CharacterSkillReuse> CharacterSkillReuses => Set<CharacterSkillReuse>();
    public DbSet<CharacterItemReuse> CharacterItemReuses => Set<CharacterItemReuse>();
    public DbSet<CharacterRecoBonus> CharacterRecoBonuses => Set<CharacterRecoBonus>();
    public DbSet<CharacterInstance> CharacterInstances => Set<CharacterInstance>();
    public DbSet<CharacterMentee> CharacterMentees => Set<CharacterMentee>();
    public DbSet<CharacterPurge> CharacterPurges => Set<CharacterPurge>();
    public DbSet<CharacterRevenge> CharacterRevenges => Set<CharacterRevenge>();
    public DbSet<CharacterSpirit> CharacterSpirits => Set<CharacterSpirit>();
    public DbSet<CharacterVariable> CharacterVariables => Set<CharacterVariable>();
    
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<PetEvolve> PetEvolves => Set<PetEvolve>();

    public DbSet<OlympiadNoble> OlympiadNobles => Set<OlympiadNoble>();
    
    public DbSet<DbItem> Items => Set<DbItem>();
    public DbSet<ItemAuction> ItemAuctions => Set<ItemAuction>();
    public DbSet<ItemAuctionBid> ItemAuctionBids => Set<ItemAuctionBid>();
    public DbSet<ItemOnGround> ItemsOnGround => Set<ItemOnGround>();
    public DbSet<DbCommissionItem> CommissionItems => Set<DbCommissionItem>();
    public DbSet<ItemTransactionHistory> ItemTransactionHistory => Set<ItemTransactionHistory>();
    public DbSet<WorldExchangeItem> WorldExchangeItems => Set<WorldExchangeItem>();
    
    public DbSet<Clan> Clans => Set<Clan>();
    public DbSet<ClanPrivileges> ClanPrivileges => Set<ClanPrivileges>();
    public DbSet<ClanSkill> ClanSkills => Set<ClanSkill>();
    public DbSet<ClanSubPledge> ClanSubPledges => Set<ClanSubPledge>();
    public DbSet<ClanNotice> ClanNotices => Set<ClanNotice>();
    public DbSet<ClanWar> ClanWars => Set<ClanWar>();
    public DbSet<PledgeApplicant> PledgeApplicants => Set<PledgeApplicant>();
    public DbSet<PledgeRecruit> PledgeRecruits => Set<PledgeRecruit>();
    public DbSet<PledgeWaitingList> PledgeWaitingLists => Set<PledgeWaitingList>();
    
    public DbSet<Ally> Allys => Set<Ally>();
    public DbSet<Crest> Crests => Set<Crest>();
    
    public DbSet<Forum> Forums => Set<Forum>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Post> Posts => Set<Post>();
    
    public DbSet<Announcement> Announcements => Set<Announcement>();

    public DbSet<BotReport> BotReports => Set<BotReport>();
    public DbSet<BufferScheme> BufferSchemes => Set<BufferScheme>();
    public DbSet<BuyList> BuyLists => Set<BuyList>();
    public DbSet<MonsterDerbyHistory> DerbyHistory => Set<MonsterDerbyHistory>();
    public DbSet<MonsterDerbyBet> DerbyBets => Set<MonsterDerbyBet>();
    public DbSet<DbAirShip> AirShips => Set<DbAirShip>();
    public DbSet<DbCastle> Castles => Set<DbCastle>();
    public DbSet<CastleManorProduction> CastleManorProduction => Set<CastleManorProduction>();
    public DbSet<CastleManorProcure> CastleManorProcure => Set<CastleManorProcure>();
    public DbSet<DbCursedWeapon> CursedWeapons => Set<DbCursedWeapon>();
    public DbSet<DbCustomMail> CustomMails => Set<DbCustomMail>();
    public DbSet<NpcRespawn> NpcRespawns => Set<NpcRespawn>();
    public DbSet<DbFort> Forts => Set<DbFort>();
    public DbSet<FortSiegeClan> FortSiegeClans => Set<FortSiegeClan>();
    public DbSet<SiegeClan> SiegeClans => Set<SiegeClan>();
    public DbSet<CastleTrapUpgrade> CastleTrapUpgrades => Set<CastleTrapUpgrade>();
    public DbSet<CastleSiegeGuard> CastleSiegeGuards => Set<CastleSiegeGuard>();
    public DbSet<HuntPass> HuntPasses => Set<HuntPass>();
    public DbSet<DbGrandBoss> GrandBosses => Set<DbGrandBoss>();
    public DbSet<DbMailMessage> MailMessages => Set<DbMailMessage>();
    public DbSet<DbPunishment> Punishments => Set<DbPunishment>();
    public DbSet<GlobalVariable> GlobalVariables => Set<GlobalVariable>();
}