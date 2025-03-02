using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

public class GameServerDbContext(DbContextOptions options)
    : DbContext(options)
{
    public DbSet<DbAccountRef> AccountRefs => Set<DbAccountRef>();
    public DbSet<DbAccountPremium> AccountPremiums => Set<DbAccountPremium>();
    public DbSet<DbAccountVariable> AccountVariables => Set<DbAccountVariable>();

    public DbSet<DbAccountCollection> AccountCollections => Set<DbAccountCollection>();
    public DbSet<DbAccountCollectionFavorite> AccountCollectionFavorites => Set<DbAccountCollectionFavorite>();

    public DbSet<DbCharacter> Characters => Set<DbCharacter>();
    public DbSet<DbCharacterFriend> CharacterFriends => Set<DbCharacterFriend>();
    public DbSet<DbCharacterSummon> CharacterSummons => Set<DbCharacterSummon>();
    public DbSet<DbCharacterOfflineTrade> CharacterOfflineTrades => Set<DbCharacterOfflineTrade>();
    public DbSet<DbCharacterOfflineTradeItem> CharacterOfflineTradeItems => Set<DbCharacterOfflineTradeItem>();
    public DbSet<DbCharacterDailyReward> CharacterDailyRewards => Set<DbCharacterDailyReward>();
    public DbSet<DbCharacterSubClass> CharacterSubClasses => Set<DbCharacterSubClass>();
    public DbSet<DbCharacterHenna> CharacterHennas => Set<DbCharacterHenna>();
    public DbSet<DbCharacterHennaPoten> CharacterHennaPotens => Set<DbCharacterHennaPoten>();
    public DbSet<DbCharacterSkill> CharacterSkills => Set<DbCharacterSkill>();
    public DbSet<DbCharacterSkillReuse> CharacterSkillReuses => Set<DbCharacterSkillReuse>();
    public DbSet<DbCharacterItemReuse> CharacterItemReuses => Set<DbCharacterItemReuse>();
    public DbSet<DbCharacterRecoBonus> CharacterRecoBonuses => Set<DbCharacterRecoBonus>();
    public DbSet<DbCharacterInstance> CharacterInstances => Set<DbCharacterInstance>();
    public DbSet<DbCharacterMentee> CharacterMentees => Set<DbCharacterMentee>();
    public DbSet<DbCharacterPurge> CharacterPurges => Set<DbCharacterPurge>();
    public DbSet<DbCharacterRevenge> CharacterRevenges => Set<DbCharacterRevenge>();
    public DbSet<DbCharacterSpirit> CharacterSpirits => Set<DbCharacterSpirit>();
    public DbSet<DbCharacterRecipeBook> CharacterRecipeBooks => Set<DbCharacterRecipeBook>();
    public DbSet<DbCharacterRecipeShopList> CharacterRecipeShopLists => Set<DbCharacterRecipeShopList>();
    public DbSet<DbCharacterShortCut> CharacterShortCuts => Set<DbCharacterShortCut>();
    public DbSet<DbCharacterTeleportBookmark> CharacterTeleportBookmarks => Set<DbCharacterTeleportBookmark>();
    public DbSet<DbCharacterRandomCraft> CharacterRandomCrafts => Set<DbCharacterRandomCraft>();
    public DbSet<DbCharacterSurveillance> CharacterSurveillances => Set<DbCharacterSurveillance>();
    public DbSet<DbCharacterPremiumItem> CharacterPremiumItems => Set<DbCharacterPremiumItem>();
    public DbSet<DbCharacterQuest> CharacterQuests => Set<DbCharacterQuest>();
    public DbSet<DbCharacterContact> CharacterContacts => Set<DbCharacterContact>();
    public DbSet<DbCharacterCouple> CharacterCouples => Set<DbCharacterCouple>();
    public DbSet<DbCharacterMacros> CharacterMacros => Set<DbCharacterMacros>();
    public DbSet<DbCharacterRankingHistory> CharacterRankingHistory => Set<DbCharacterRankingHistory>();
    public DbSet<DbCharacterVariable> CharacterVariables => Set<DbCharacterVariable>();

    public DbSet<DbHero> Heroes => Set<DbHero>();
    public DbSet<DbHeroDiary> HeroesDiary => Set<DbHeroDiary>();
    public DbSet<DbOlympiadData> OlympiadData => Set<DbOlympiadData>();
    public DbSet<DbOlympiadFight> OlympiadFights => Set<DbOlympiadFight>();
    public DbSet<DbOlympiadNoble> OlympiadNobles => Set<DbOlympiadNoble>();
    public DbSet<DbOlympiadNobleEom> OlympiadNoblesEom => Set<DbOlympiadNobleEom>();


    public DbSet<DbPet> Pets => Set<DbPet>();
    public DbSet<DbPetEvolve> PetEvolves => Set<DbPetEvolve>();
    public DbSet<DbPetSkill> PetSkills => Set<DbPetSkill>();
    public DbSet<DbPetSkillReuse> PetSkillReuses => Set<DbPetSkillReuse>();

    public DbSet<DbSummonSkillReuse> SummonSkillReuses => Set<DbSummonSkillReuse>();


    public DbSet<DbItem> Items => Set<DbItem>();
    public DbSet<DbItemVariation> ItemVariations => Set<DbItemVariation>();
    public DbSet<DbItemElemental> ItemElementals => Set<DbItemElemental>();
    public DbSet<DbItemAuction> ItemAuctions => Set<DbItemAuction>();
    public DbSet<DbItemAuctionBid> ItemAuctionBids => Set<DbItemAuctionBid>();
    public DbSet<DbItemOnGround> ItemsOnGround => Set<DbItemOnGround>();
    public DbSet<DbCommissionItem> CommissionItems => Set<DbCommissionItem>();
    public DbSet<DbItemTransactionHistory> ItemTransactionHistory => Set<DbItemTransactionHistory>();
    public DbSet<DbItemVariable> ItemVariables => Set<DbItemVariable>();
    public DbSet<DbItemSpecialAbility> ItemSpecialAbilities => Set<DbItemSpecialAbility>();
    public DbSet<DbWorldExchangeItem> WorldExchangeItems => Set<DbWorldExchangeItem>();

    public DbSet<DbNpcVariable> NpcVariables => Set<DbNpcVariable>();

    public DbSet<DbClan> Clans => Set<DbClan>();
    public DbSet<DbClanPrivileges> ClanPrivileges => Set<DbClanPrivileges>();
    public DbSet<DbClanSkill> ClanSkills => Set<DbClanSkill>();
    public DbSet<DbClanSubPledge> ClanSubPledges => Set<DbClanSubPledge>();
    public DbSet<DbClanNotice> ClanNotices => Set<DbClanNotice>();
    public DbSet<DbClanWar> ClanWars => Set<DbClanWar>();
    public DbSet<DbClanVariable> ClanVariables => Set<DbClanVariable>();
    public DbSet<DbClanHall> ClanHalls => Set<DbClanHall>();
    public DbSet<DbClanHallBidder> ClanHallBidders => Set<DbClanHallBidder>();
    public DbSet<DbPledgeApplicant> PledgeApplicants => Set<DbPledgeApplicant>();
    public DbSet<DbPledgeRecruit> PledgeRecruits => Set<DbPledgeRecruit>();
    public DbSet<DbPledgeWaitingList> PledgeWaitingLists => Set<DbPledgeWaitingList>();

    public DbSet<DbAlly> Allies => Set<DbAlly>();
    public DbSet<DbCrest> Crests => Set<DbCrest>();

    public DbSet<DbForum> Forums => Set<DbForum>();
    public DbSet<DbTopic> Topics => Set<DbTopic>();
    public DbSet<DbPost> Posts => Set<DbPost>();

    public DbSet<DbAnnouncement> Announcements => Set<DbAnnouncement>();

    public DbSet<DbBotReport> BotReports => Set<DbBotReport>();
    public DbSet<DbBufferScheme> BufferSchemes => Set<DbBufferScheme>();
    public DbSet<DbMonsterDerbyHistory> DerbyHistory => Set<DbMonsterDerbyHistory>();
    public DbSet<DbMonsterDerbyBet> DerbyBets => Set<DbMonsterDerbyBet>();
    public DbSet<DbAirShip> AirShips => Set<DbAirShip>();

    public DbSet<DbCastle> Castles => Set<DbCastle>();
    public DbSet<DbCastleFunction> CastleFunctions => Set<DbCastleFunction>();
    public DbSet<DbCastleDoorUpgrade> CastleDoorUpgrades => Set<DbCastleDoorUpgrade>();
    public DbSet<DbCastleManorProduction> CastleManorProduction => Set<DbCastleManorProduction>();
    public DbSet<CastleManorProcure> CastleManorProcure => Set<CastleManorProcure>();
    public DbSet<DbCastleTrapUpgrade> CastleTrapUpgrades => Set<DbCastleTrapUpgrade>();
    public DbSet<DbCastleSiegeGuard> CastleSiegeGuards => Set<DbCastleSiegeGuard>();
    public DbSet<DbCastleSiegeClan> CastleSiegeClans => Set<DbCastleSiegeClan>();

    public DbSet<DbFort> Forts => Set<DbFort>();
    public DbSet<DbFortFunction> FortFunctions => Set<DbFortFunction>();
    public DbSet<DbFortDoorUpgrade> FortDoorUpgrades => Set<DbFortDoorUpgrade>();
    public DbSet<DbFortSpawn> FortSpawns => Set<DbFortSpawn>();
    public DbSet<DbFortSiegeGuard> FortSiegeGuards => Set<DbFortSiegeGuard>();

    public DbSet<DbCursedWeapon> CursedWeapons => Set<DbCursedWeapon>();
    public DbSet<DbCustomMail> CustomMails => Set<DbCustomMail>();
    public DbSet<DbNpcRespawn> NpcRespawns => Set<DbNpcRespawn>();
    public DbSet<DbFortSiegeClan> FortSiegeClans => Set<DbFortSiegeClan>();
    public DbSet<DbSiegeClan> SiegeClans => Set<DbSiegeClan>();
    public DbSet<DbHuntPass> HuntPasses => Set<DbHuntPass>();
    public DbSet<DbGrandBoss> GrandBosses => Set<DbGrandBoss>();
    public DbSet<DbMailMessage> MailMessages => Set<DbMailMessage>();
    public DbSet<DbPunishment> Punishments => Set<DbPunishment>();
    public DbSet<DbGlobalVariable> GlobalVariables => Set<DbGlobalVariable>();
    public DbSet<DbBuyList> BuyLists => Set<DbBuyList>();

    public DbSet<DbPartyMatchingHistory> PartyMatchingHistory => Set<DbPartyMatchingHistory>();

    public DbSet<DbResidenceFunction> ResidenceFunctions => Set<DbResidenceFunction>();
    public DbSet<DbAchievementBox> AchievementBoxes => Set<DbAchievementBox>();
    public DbSet<DbEnchantChallengePoint> EnchantChallengePoints => Set<DbEnchantChallengePoint>();
    public DbSet<DbEnchantChallengePointRecharge> EnchantChallengePointRecharges => Set<DbEnchantChallengePointRecharge>();
    public DbSet<DbGlobalTask> GlobalTasks => Set<DbGlobalTask>();
    public DbSet<DbMerchantLease> MerchantLeases => Set<DbMerchantLease>();

    public DbSet<DbPetitionFeedback> PetitionFeedbacks => Set<DbPetitionFeedback>();

    public DbSet<DbBbsFavorite> BbsFavorites => Set<DbBbsFavorite>();
}