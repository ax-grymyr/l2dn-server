using L2Dn.Configuration;
using NLog;

namespace L2Dn.GameServer.Configuration;

/// <summary>
/// This class loads all the game server related configurations from files.
/// The files are usually located in config folder in server root folder.
/// Each configuration has a default value (that should reflect retail behavior).
/// </summary>
public static partial class Config
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Config));

	// --------------------------------------------------
	// Config File Definitions
	// --------------------------------------------------
	public const string FORTSIEGE_CONFIG_FILE = "./Config/FortSiege.ini";
	public const string SIEGE_CONFIG_FILE = "./Config/Siege.ini";

    // --------------------------------------------------
	// General Settings
	// --------------------------------------------------
	public static bool SHOW_LICENCE;
	public static bool SHOW_PI_AGREEMENT;
	public static bool ACCEPT_NEW_GAMESERVER;
	public static int SERVER_ID;
	public static byte[] HEX_ID = [];
	public static bool AUTO_CREATE_ACCOUNTS;
	public static bool FLOOD_PROTECTION;
	public static int FAST_CONNECTION_LIMIT;
	public static int NORMAL_CONNECTION_TIME;
	public static int FAST_CONNECTION_TIME;
	public static int MAX_CONNECTION_PER_IP;
	public static bool ENABLE_CMD_LINE_LOGIN;
	public static bool ONLY_CMD_LINE_LOGIN;

	// --------------------------------------------------
	// Custom Settings
	// --------------------------------------------------

    internal static void Load(string configPath)
    {
        ConfigurationParser parser = new(configPath);

        Server.Load(parser);
        Feature.Load(parser);
        Attendance.Load(parser);
        AttributeSystem.Load(parser);
        Character.Load(parser);
        MagicLamp.Load(parser);
        RandomCraft.Load(parser);
        WorldExchange.Load(parser);
        TrainingCamp.Load(parser);
        GameAssistant.Load(parser);
        General.Load(parser);
        FloodProtector.Load(parser);
        Npc.Load(parser);
        Rates.Load(parser);
        Pvp.Load(parser);
        Olympiad.Load(parser);
        GrandBoss.Load(parser);
        HuntPass.Load(parser);
        AchievementBox.Load(parser);
        OrcFortress.Load(parser);
        GraciaSeeds.Load(parser);
        GeoEngine.Load(parser);
        ChatFilter.LoadFilter(configPath);

        // Custom
        AllowedPlayerRaces.Load(parser);
        AutoPotions.Load(parser);
        Banking.Load(parser);
        BossAnnouncements.Load(parser);
        NpcStatMultipliers.Load(parser);
        ChampionMonsters.Load(parser);
        ChatModeration.Load(parser);
        ClassBalance.Load(parser);
        CommunityBoard.Load(parser);
        CustomDepositableItems.Load(parser);
        CustomMailManager.Load(parser);
        DelevelManager.Load(parser);
        DualboxCheck.Load(parser);
        FactionSystem.Load(parser);
        FakePlayers.Load(parser);
        FindPvP.Load(parser);
        MerchantZeroSellPrice.Load(parser);
        MultilingualSupport.Load(parser);
        NoblessMaster.Load(parser);
        OfflinePlay.Load(parser);
        OfflineTrade.Load(parser);
        OnlineInfo.Load(parser);
        PasswordChange.Load(parser);
        VipSystem.Load(parser);
        PremiumSystem.Load(parser);
        PrivateStoreRange.Load(parser);
        PvpAnnounce.Load(parser);
        PvpRewardItem.Load(parser);
        PvpTitleColor.Load(parser);
        RandomSpawns.Load(parser);
        SayuneForAll.Load(parser);
        ScreenWelcomeMessage.Load(parser);
        SellBuffs.Load(parser);
        ServerTime.Load(parser);
        SchemeBuffer.Load(parser);
        StartingLocation.Load(parser);
        WalkerBotProtection.Load(parser);
    }
}