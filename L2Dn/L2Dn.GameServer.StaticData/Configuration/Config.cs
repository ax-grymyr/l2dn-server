using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using L2Dn.Configuration;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Configuration;

/// <summary>
/// This class loads all the game server related configurations from files.
/// The files are usually located in config folder in server root folder.
/// Each configuration has a default value (that should reflect retail behavior).
/// </summary>
public static partial class Config
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Config));

	// --------------------------------------------------
	// Config File Definitions
	// --------------------------------------------------
	public const string OLYMPIAD_CONFIG_FILE = "./Config/Olympiad.ini";

	public const string FORTSIEGE_CONFIG_FILE = "./Config/FortSiege.ini";
	public const string SIEGE_CONFIG_FILE = "./Config/Siege.ini";

	private const string FLOOD_PROTECTOR_CONFIG_FILE = "./Config/FloodProtector.ini";
	private const string GAME_ASSISTANT_CONFIG_FILE = "./Config/GameAssistant.ini";
	private const string GENERAL_CONFIG_FILE = "./Config/General.ini";
	private const string GEOENGINE_CONFIG_FILE = "./Config/GeoEngine.ini";
	private const string GRACIASEEDS_CONFIG_FILE = "./Config/GraciaSeeds.ini";
	private const string GRANDBOSS_CONFIG_FILE = "./Config/GrandBoss.ini";
	private const string HUNT_PASS_CONFIG_FILE = "./Config/HuntPass.ini";
	private const string ACHIEVEMENT_BOX_CONFIG_FILE = "./Config/AchievementBox.ini";
	private const string MAGIC_LAMP_FILE = "./Config/MagicLamp.ini";
	private const string NPC_CONFIG_FILE = "./Config/NPC.ini";
	private const string ORC_FORTRESS_CONFIG_FILE = "./Config/OrcFortress.ini";
	private const string PVP_CONFIG_FILE = "./Config/PVP.ini";
	private const string RANDOM_CRAFT_FILE = "./Config/RandomCraft.ini";
	private const string RATES_CONFIG_FILE = "./Config/Rates.ini";
	private const string TRAINING_CAMP_CONFIG_FILE = "./Config/TrainingCamp.ini";
	private const string WORLD_EXCHANGE_FILE = "./Config/WorldExchange.ini";

	private const string CHAT_FILTER_FILE = "./Config/chatfilter.txt";

	// --------------------------------------------------
	// Custom Config File Definitions
	// --------------------------------------------------
	private const string CUSTOM_ALLOWED_PLAYER_RACES_CONFIG_FILE = "./Config/Custom/AllowedPlayerRaces.ini";
	private const string CUSTOM_AUTO_POTIONS_CONFIG_FILE = "./Config/Custom/AutoPotions.ini";
	private const string CUSTOM_BANKING_CONFIG_FILE = "./Config/Custom/Banking.ini";
	private const string CUSTOM_BOSS_ANNOUNCEMENTS_CONFIG_FILE = "./Config/Custom/BossAnnouncements.ini";
	private const string CUSTOM_CHAMPION_MONSTERS_CONFIG_FILE = "./Config/Custom/ChampionMonsters.ini";
	private const string CUSTOM_CHAT_MODERATION_CONFIG_FILE = "./Config/Custom/ChatModeration.ini";
	private const string CUSTOM_CLASS_BALANCE_CONFIG_FILE = "./Config/Custom/ClassBalance.ini";
	private const string CUSTOM_COMMUNITY_BOARD_CONFIG_FILE = "./Config/Custom/CommunityBoard.ini";
	private const string CUSTOM_CUSTOM_DEPOSITABLE_ITEMS_CONFIG_FILE = "./Config/Custom/CustomDepositableItems.ini";
	private const string CUSTOM_CUSTOM_MAIL_MANAGER_CONFIG_FILE = "./Config/Custom/CustomMailManager.ini";
	private const string CUSTOM_DELEVEL_MANAGER_CONFIG_FILE = "./Config/Custom/DelevelManager.ini";
	private const string CUSTOM_DUALBOX_CHECK_CONFIG_FILE = "./Config/Custom/DualboxCheck.ini";
	private const string CUSTOM_FACTION_SYSTEM_CONFIG_FILE = "./Config/Custom/FactionSystem.ini";
	private const string CUSTOM_FAKE_PLAYERS_CONFIG_FILE = "./Config/Custom/FakePlayers.ini";
	private const string CUSTOM_FIND_PVP_CONFIG_FILE = "./Config/Custom/FindPvP.ini";
	private const string CUSTOM_MERCHANT_ZERO_SELL_PRICE_CONFIG_FILE = "./Config/Custom/MerchantZeroSellPrice.ini";
	private const string CUSTOM_MULTILANGUAL_SUPPORT_CONFIG_FILE = "./Config/Custom/MultilingualSupport.ini";
	private const string CUSTOM_NOBLESS_MASTER_CONFIG_FILE = "./Config/Custom/NoblessMaster.ini";
	private const string CUSTOM_NPC_STAT_MULTIPLIERS_CONFIG_FILE = "./Config/Custom/NpcStatMultipliers.ini";
	private const string CUSTOM_OFFLINE_PLAY_CONFIG_FILE = "./Config/Custom/OfflinePlay.ini";
	private const string CUSTOM_OFFLINE_TRADE_CONFIG_FILE = "./Config/Custom/OfflineTrade.ini";
	private const string CUSTOM_ONLINE_INFO_CONFIG_FILE = "./Config/Custom/OnlineInfo.ini";
	private const string CUSTOM_PASSWORD_CHANGE_CONFIG_FILE = "./Config/Custom/PasswordChange.ini";
	private const string CUSTOM_VIP_CONFIG_FILE = "./Config/Custom/VipSystem.ini";
	private const string CUSTOM_PREMIUM_SYSTEM_CONFIG_FILE = "./Config/Custom/PremiumSystem.ini";
	private const string CUSTOM_PRIVATE_STORE_RANGE_CONFIG_FILE = "./Config/Custom/PrivateStoreRange.ini";
	private const string CUSTOM_PVP_ANNOUNCE_CONFIG_FILE = "./Config/Custom/PvpAnnounce.ini";
	private const string CUSTOM_PVP_REWARD_ITEM_CONFIG_FILE = "./Config/Custom/PvpRewardItem.ini";
	private const string CUSTOM_PVP_TITLE_CONFIG_FILE = "./Config/Custom/PvpTitleColor.ini";
	private const string CUSTOM_RANDOM_SPAWNS_CONFIG_FILE = "./Config/Custom/RandomSpawns.ini";
	private const string CUSTOM_SAYUNE_FOR_ALL_CONFIG_FILE = "./Config/Custom/SayuneForAll.ini";
	private const string CUSTOM_SCREEN_WELCOME_MESSAGE_CONFIG_FILE = "./Config/Custom/ScreenWelcomeMessage.ini";
	private const string CUSTOM_SELL_BUFFS_CONFIG_FILE = "./Config/Custom/SellBuffs.ini";
	private const string CUSTOM_SERVER_TIME_CONFIG_FILE = "./Config/Custom/ServerTime.ini";
	private const string CUSTOM_SCHEME_BUFFER_CONFIG_FILE = "./Config/Custom/SchemeBuffer.ini";
	private const string CUSTOM_STARTING_LOCATION_CONFIG_FILE = "./Config/Custom/StartingLocation.ini";
	private const string CUSTOM_WALKER_BOT_PROTECTION_CONFIG_FILE = "./Config/Custom/WalkerBotProtection.ini";

	// --------------------------------------------------
	// Variable Definitions
	// --------------------------------------------------

	// --------------------------------------------------
	// Fortress Settings
	// --------------------------------------------------
    public static bool ORC_FORTRESS_ENABLE;
    public static TimeOnly ORC_FORTRESS_TIME;

	// --------------------------------------------------
	// General Settings
	// --------------------------------------------------
	public static int DEFAULT_ACCESS_LEVEL;
	public static bool SERVER_GMONLY;
	public static bool GM_HERO_AURA;
	public static bool GM_STARTUP_BUILDER_HIDE;
	public static bool GM_STARTUP_INVULNERABLE;
	public static bool GM_STARTUP_INVISIBLE;
	public static bool GM_STARTUP_SILENCE;
	public static bool GM_STARTUP_AUTO_LIST;
	public static bool GM_STARTUP_DIET_MODE;
	public static bool GM_ITEM_RESTRICTION;
	public static bool GM_SKILL_RESTRICTION;
	public static bool GM_TRADE_RESTRICTED_ITEMS;
	public static bool GM_RESTART_FIGHTING;
	public static bool GM_ANNOUNCER_NAME;
	public static bool GM_GIVE_SPECIAL_SKILLS;
	public static bool GM_GIVE_SPECIAL_AURA_SKILLS;
	public static bool GM_DEBUG_HTML_PATHS;
	public static bool USE_SUPER_HASTE_AS_GM_SPEED;
	public static bool LOG_CHAT;
	public static bool LOG_AUTO_ANNOUNCEMENTS;
	public static bool LOG_ITEMS;
	public static bool LOG_ITEMS_SMALL_LOG;
	public static bool LOG_ITEMS_IDS_ONLY;
	public static ImmutableSortedSet<int> LOG_ITEMS_IDS_LIST = ImmutableSortedSet<int>.Empty;
	public static bool LOG_ITEM_ENCHANTS;
	public static bool LOG_SKILL_ENCHANTS;
	public static bool GMAUDIT;
	public static bool SKILL_CHECK_ENABLE;
	public static bool SKILL_CHECK_REMOVE;
	public static bool SKILL_CHECK_GM;
	public static bool HTML_ACTION_CACHE_DEBUG;
	public static bool DEVELOPER;
	public static bool ALT_DEV_NO_QUESTS;
	public static bool ALT_DEV_NO_SPAWNS;
	public static bool ALT_DEV_SHOW_QUESTS_LOAD_IN_LOGS;
	public static bool ALT_DEV_SHOW_SCRIPTS_LOAD_IN_LOGS;
	public static bool DEBUG_CLIENT_PACKETS;
	public static bool DEBUG_EX_CLIENT_PACKETS;
	public static bool DEBUG_SERVER_PACKETS;
	public static bool DEBUG_UNKNOWN_PACKETS;
	public static ImmutableSortedSet<string> ALT_DEV_EXCLUDED_PACKETS = ImmutableSortedSet<string>.Empty;
	public static bool ALLOW_DISCARDITEM;
	public static int AUTODESTROY_ITEM_AFTER;
	public static int HERB_AUTO_DESTROY_TIME;
	public static ImmutableSortedSet<int> LIST_PROTECTED_ITEMS = ImmutableSortedSet<int>.Empty;
	public static bool DATABASE_CLEAN_UP;
	public static int CHAR_DATA_STORE_INTERVAL;
	public static int CLAN_VARIABLES_STORE_INTERVAL;
	public static bool LAZY_ITEMS_UPDATE;
	public static bool UPDATE_ITEMS_ON_CHAR_STORE;
	public static bool DESTROY_DROPPED_PLAYER_ITEM;
	public static bool DESTROY_EQUIPABLE_PLAYER_ITEM;
	public static bool DESTROY_ALL_ITEMS;
	public static bool SAVE_DROPPED_ITEM;
	public static bool EMPTY_DROPPED_ITEM_TABLE_AFTER_LOAD;
	public static int SAVE_DROPPED_ITEM_INTERVAL;
	public static bool CLEAR_DROPPED_ITEM_TABLE;
	public static bool ORDER_QUEST_LIST_BY_QUESTID;
	public static bool AUTODELETE_INVALID_QUEST_DATA;
	public static bool ENABLE_STORY_QUEST_BUFF_REWARD;
	public static bool MULTIPLE_ITEM_DROP;
	public static bool HTM_CACHE;
	public static bool CHECK_HTML_ENCODING;
	public static bool HIDE_BYPASS_REMOVAL;
	public static int MIN_NPC_ANIMATION;
	public static int MAX_NPC_ANIMATION;
	public static int MIN_MONSTER_ANIMATION;
	public static int MAX_MONSTER_ANIMATION;
	public static bool CORRECT_PRICES;
	public static bool ENABLE_FALLING_DAMAGE;
	public static bool GRIDS_ALWAYS_ON;
	public static int GRID_NEIGHBOR_TURNON_TIME;
	public static int GRID_NEIGHBOR_TURNOFF_TIME;
	public static int PEACE_ZONE_MODE;
	public static string DEFAULT_GLOBAL_CHAT = "ON";
	public static string DEFAULT_TRADE_CHAT = "ON";
	public static bool ENABLE_WORLD_CHAT;
	public static int MINIMUM_CHAT_LEVEL;
	public static bool ALLOW_WAREHOUSE;
	public static bool ALLOW_REFUND;
	public static bool ALLOW_MAIL;
	public static bool ALLOW_ATTACHMENTS;
	public static bool ALLOW_WEAR;
	public static int WEAR_DELAY;
	public static int WEAR_PRICE;
	public static int STORE_REVIEW_LIMIT;
	public static int STORE_REVIEW_CACHE_TIME;
	public static int INSTANCE_FINISH_TIME;
	public static bool RESTORE_PLAYER_INSTANCE;
	public static int EJECT_DEAD_PLAYER_TIME;
	public static bool ALLOW_RACE;
	public static bool ALLOW_WATER;
	public static bool ALLOW_FISHING;
	public static bool ALLOW_BOAT;
	public static int BOAT_BROADCAST_RADIUS;
	public static bool ALLOW_CURSED_WEAPONS;
	public static bool ALLOW_MANOR;
	public static bool SERVER_NEWS;
	public static bool ENABLE_COMMUNITY_BOARD;
	public static string BBS_DEFAULT = "_bbshome";
	public static bool USE_SAY_FILTER;
	public static string CHAT_FILTER_CHARS = "^_^";
	public static ImmutableSortedSet<ChatType> BAN_CHAT_CHANNELS = ImmutableSortedSet<ChatType>.Empty;
	public static int WORLD_CHAT_MIN_LEVEL;
	public static int WORLD_CHAT_POINTS_PER_DAY;
	public static TimeSpan WORLD_CHAT_INTERVAL;
	public static bool OLYMPIAD_ENABLED;
	public static int ALT_OLY_START_TIME;
	public static int ALT_OLY_MIN;
	public static long ALT_OLY_CPERIOD;
	public static long ALT_OLY_BATTLE;
	public static long ALT_OLY_WPERIOD;
	public static long ALT_OLY_VPERIOD;
	public static int ALT_OLY_START_POINTS;
	public static int ALT_OLY_WEEKLY_POINTS;
	public static int ALT_OLY_CLASSED;
	public static int ALT_OLY_NONCLASSED;
	public static ImmutableArray<ItemHolder> ALT_OLY_WINNER_REWARD = ImmutableArray<ItemHolder>.Empty;
	public static ImmutableArray<ItemHolder> ALT_OLY_LOSER_REWARD = ImmutableArray<ItemHolder>.Empty;
	public static int ALT_OLY_COMP_RITEM;
	public static int ALT_OLY_MIN_MATCHES;
	public static int ALT_OLY_MARK_PER_POINT;
	public static int ALT_OLY_HERO_POINTS;
	public static int ALT_OLY_RANK1_POINTS;
	public static int ALT_OLY_RANK2_POINTS;
	public static int ALT_OLY_RANK3_POINTS;
	public static int ALT_OLY_RANK4_POINTS;
	public static int ALT_OLY_RANK5_POINTS;
	public static int ALT_OLY_MAX_POINTS;
	public static int ALT_OLY_DIVIDER_CLASSED;
	public static int ALT_OLY_DIVIDER_NON_CLASSED;
	public static int ALT_OLY_MAX_WEEKLY_MATCHES;
	public static bool ALT_OLY_LOG_FIGHTS;
	public static bool ALT_OLY_SHOW_MONTHLY_WINNERS;
	public static bool ALT_OLY_ANNOUNCE_GAMES;
	public static ImmutableSortedSet<int> LIST_OLY_RESTRICTED_ITEMS = ImmutableSortedSet<int>.Empty;
	public static int ALT_OLY_WEAPON_ENCHANT_LIMIT;
	public static int ALT_OLY_ARMOR_ENCHANT_LIMIT;
	public static int ALT_OLY_WAIT_TIME;
	public static string ALT_OLY_PERIOD = "MONTH";
	public static int ALT_OLY_PERIOD_MULTIPLIER;
	public static ImmutableSortedSet<DayOfWeek> ALT_OLY_COMPETITION_DAYS = ImmutableSortedSet<DayOfWeek>.Empty;
	public static int ALT_MANOR_REFRESH_TIME;
	public static int ALT_MANOR_REFRESH_MIN;
	public static int ALT_MANOR_APPROVE_TIME;
	public static int ALT_MANOR_APPROVE_MIN;
	public static int ALT_MANOR_MAINTENANCE_MIN;
	public static bool ALT_MANOR_SAVE_ALL_ACTIONS;
	public static int ALT_MANOR_SAVE_PERIOD_RATE;
	public static bool ALT_ITEM_AUCTION_ENABLED;
	public static int ALT_ITEM_AUCTION_EXPIRED_AFTER;
	public static long ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID;
	public static IllegalActionPunishmentType DEFAULT_PUNISH;
	public static long DEFAULT_PUNISH_PARAM;
	public static bool ONLY_GM_ITEMS_FREE;
	public static bool JAIL_IS_PVP;
	public static bool JAIL_DISABLE_CHAT;
	public static bool JAIL_DISABLE_TRANSACTION;
	public static bool CUSTOM_NPC_DATA;
	public static bool CUSTOM_TELEPORT_TABLE;
	public static bool CUSTOM_SKILLS_LOAD;
	public static bool CUSTOM_ITEMS_LOAD;
	public static bool CUSTOM_MULTISELL_LOAD;
	public static bool CUSTOM_BUYLIST_LOAD;
	public static int BOOKMARK_CONSUME_ITEM_ID;
	public static int ALT_BIRTHDAY_GIFT;
	public static string ALT_BIRTHDAY_MAIL_SUBJECT = "Happy Birthday!";

    public static string ALT_BIRTHDAY_MAIL_TEXT =
        "Hello Adventurer!! Seeing as you're one year older now, I thought I would send you some birthday cheer :) " +
        "Please find your birthday pack attached. May these gifts bring you joy and happiness on this very special day." +
        Environment.NewLine + Environment.NewLine + "Sincerely, Alegria";

	public static bool BOTREPORT_ENABLE;
	public static TimeOnly BOTREPORT_RESETPOINT_HOUR;
	public static long BOTREPORT_REPORT_DELAY;
	public static bool BOTREPORT_ALLOW_REPORTS_FROM_SAME_CLAN_MEMBERS;
	public static bool ENABLE_AUTO_PLAY;
	public static bool ENABLE_AUTO_POTION;
	public static bool ENABLE_AUTO_PET_POTION;
	public static bool ENABLE_AUTO_SKILL;
	public static bool ENABLE_AUTO_ITEM;
	public static bool AUTO_PLAY_ATTACK_ACTION;
	public static bool RESUME_AUTO_PLAY;
	public static bool ENABLE_AUTO_ASSIST;
	public static AbnormalVisualEffect BLUE_TEAM_ABNORMAL_EFFECT;
	public static AbnormalVisualEffect RED_TEAM_ABNORMAL_EFFECT;
	public static int SHARING_LOCATION_COST;
	public static int TELEPORT_SHARE_LOCATION_COST;

	// --------------------------------------------------
	// FloodProtector Settings
	// --------------------------------------------------
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_USE_ITEM = new("UseItemFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_ROLL_DICE = new("RollDiceFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_ITEM_PET_SUMMON = new("ItemPetSummonFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_HERO_VOICE = new("HeroVoiceFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_GLOBAL_CHAT = new("GlobalChatFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_SUBCLASS = new("SubclassFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_DROP_ITEM = new("DropItemFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_SERVER_BYPASS = new("ServerBypassFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_MULTISELL = new("MultiSellFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_TRANSACTION = new("TransactionFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_MANUFACTURE = new("ManufactureFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_SENDMAIL = new("SendMailFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_CHARACTER_SELECT = new("CharacterSelectFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_ITEM_AUCTION = new("ItemAuctionFloodProtector");
	public static readonly FloodProtectorConfig FLOOD_PROTECTOR_PLAYER_ACTION = new("PlayerActionFloodProtector");

	// --------------------------------------------------
	// NPC Settings
	// --------------------------------------------------
	public static bool ANNOUNCE_MAMMON_SPAWN;
	public static bool ALT_MOB_AGRO_IN_PEACEZONE;
	public static bool ALT_ATTACKABLE_NPCS;
	public static bool ALT_GAME_VIEWNPC;
	public static bool SHOW_NPC_LEVEL;
	public static bool SHOW_NPC_AGGRESSION;
	public static bool ATTACKABLES_CAMP_PLAYER_CORPSES;
	public static bool SHOW_CREST_WITHOUT_QUEST;
	public static bool ENABLE_RANDOM_ENCHANT_EFFECT;
	public static int MIN_NPC_LEVEL_DMG_PENALTY;
	public static ImmutableArray<double> NPC_DMG_PENALTY = ImmutableArray<double>.Empty;
	public static ImmutableArray<double> NPC_CRIT_DMG_PENALTY = ImmutableArray<double>.Empty;
	public static ImmutableArray<double> NPC_SKILL_DMG_PENALTY = ImmutableArray<double>.Empty;
	public static int MIN_NPC_LEVEL_MAGIC_PENALTY;
	public static ImmutableArray<double> NPC_SKILL_CHANCE_PENALTY = ImmutableArray<double>.Empty;
	public static int DEFAULT_CORPSE_TIME;
	public static int SPOILED_CORPSE_EXTEND_TIME;
	public static TimeSpan CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY;
	public static int MAX_DRIFT_RANGE;
	public static bool AGGRO_DISTANCE_CHECK_ENABLED;
	public static int AGGRO_DISTANCE_CHECK_RANGE;
	public static bool AGGRO_DISTANCE_CHECK_RAIDS;
	public static int AGGRO_DISTANCE_CHECK_RAID_RANGE;
	public static bool AGGRO_DISTANCE_CHECK_INSTANCES;
	public static bool AGGRO_DISTANCE_CHECK_RESTORE_LIFE;
	public static bool GUARD_ATTACK_AGGRO_MOB;
	public static double RAID_HP_REGEN_MULTIPLIER;
	public static double RAID_MP_REGEN_MULTIPLIER;
	public static double RAID_PDEFENCE_MULTIPLIER;
	public static double RAID_MDEFENCE_MULTIPLIER;
	public static double RAID_PATTACK_MULTIPLIER;
	public static double RAID_MATTACK_MULTIPLIER;
	public static double RAID_MINION_RESPAWN_TIMER;
	public static ImmutableDictionary<int, int> MINIONS_RESPAWN_TIME = ImmutableDictionary<int, int>.Empty;
	public static float RAID_MIN_RESPAWN_MULTIPLIER;
	public static float RAID_MAX_RESPAWN_MULTIPLIER;
	public static bool RAID_DISABLE_CURSE;
	public static bool FORCE_DELETE_MINIONS;
	public static int RAID_CHAOS_TIME;
	public static int GRAND_CHAOS_TIME;
	public static int MINION_CHAOS_TIME;
	public static int INVENTORY_MAXIMUM_PET;
	public static double PET_HP_REGEN_MULTIPLIER;
	public static double PET_MP_REGEN_MULTIPLIER;
	public static int VITALITY_CONSUME_BY_MOB;
	public static int VITALITY_CONSUME_BY_BOSS;

	// --------------------------------------------------
	// PvP Settings
	// --------------------------------------------------
	public static bool KARMA_DROP_GM;
	public static int KARMA_PK_LIMIT;
	public static ImmutableSortedSet<int> KARMA_NONDROPPABLE_PET_ITEMS = ImmutableSortedSet<int>.Empty;
	public static ImmutableSortedSet<int> KARMA_NONDROPPABLE_ITEMS = ImmutableSortedSet<int>.Empty;
	public static bool ANTIFEED_ENABLE;
	public static bool ANTIFEED_DUALBOX;
	public static bool ANTIFEED_DISCONNECTED_AS_DUALBOX;
	public static int ANTIFEED_INTERVAL;
	public static bool VAMPIRIC_ATTACK_AFFECTS_PVP;
	public static bool MP_VAMPIRIC_ATTACK_AFFECTS_PVP;

	// --------------------------------------------------
	// Rate Settings
	// --------------------------------------------------
	public static float RATE_XP;
	public static float RATE_SP;
	public static float RATE_PARTY_XP;
	public static float RATE_PARTY_SP;
	public static float RATE_INSTANCE_XP;
	public static float RATE_INSTANCE_SP;
	public static float RATE_INSTANCE_PARTY_XP;
	public static float RATE_INSTANCE_PARTY_SP;
	public static float RATE_RAIDBOSS_POINTS;
	public static float RATE_EXTRACTABLE;
	public static int RATE_DROP_MANOR;
	public static float RATE_QUEST_DROP;
	public static float RATE_QUEST_REWARD;
	public static float RATE_QUEST_REWARD_XP;
	public static float RATE_QUEST_REWARD_SP;
	public static float RATE_QUEST_REWARD_ADENA;
	public static bool RATE_QUEST_REWARD_USE_MULTIPLIERS;
	public static float RATE_QUEST_REWARD_POTION;
	public static float RATE_QUEST_REWARD_SCROLL;
	public static float RATE_QUEST_REWARD_RECIPE;
	public static float RATE_QUEST_REWARD_MATERIAL;
	public static float RATE_DEATH_DROP_AMOUNT_MULTIPLIER;
	public static float RATE_SPOIL_DROP_AMOUNT_MULTIPLIER;
	public static float RATE_HERB_DROP_AMOUNT_MULTIPLIER;
	public static float RATE_RAID_DROP_AMOUNT_MULTIPLIER;
	public static float RATE_DEATH_DROP_CHANCE_MULTIPLIER;
	public static float RATE_SPOIL_DROP_CHANCE_MULTIPLIER;
	public static float RATE_HERB_DROP_CHANCE_MULTIPLIER;
	public static float RATE_RAID_DROP_CHANCE_MULTIPLIER;
	public static ImmutableDictionary<int, float> RATE_DROP_AMOUNT_BY_ID = ImmutableDictionary<int, float>.Empty;
	public static ImmutableDictionary<int, float> RATE_DROP_CHANCE_BY_ID = ImmutableDictionary<int, float>.Empty;
	public static int DROP_MAX_OCCURRENCES_NORMAL;
	public static int DROP_MAX_OCCURRENCES_RAIDBOSS;
	public static int DROP_ADENA_MAX_LEVEL_LOWEST_DIFFERENCE;
	public static int DROP_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE;
	public static int EVENT_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE;
	public static double BLESSING_CHANCE;
	public static bool BOSS_DROP_ENABLED;
	public static int BOSS_DROP_MIN_LEVEL;
	public static int BOSS_DROP_MAX_LEVEL;
	public static ImmutableArray<DropHolder> BOSS_DROP_LIST = ImmutableArray<DropHolder>.Empty;
	public static bool LCOIN_DROP_ENABLED;
	public static double LCOIN_DROP_CHANCE;
	public static int LCOIN_MIN_MOB_LEVEL;
	public static int LCOIN_MIN_QUANTITY;
	public static int LCOIN_MAX_QUANTITY;
	public static float RATE_KARMA_LOST;
	public static float RATE_KARMA_EXP_LOST;
	public static float RATE_SIEGE_GUARDS_PRICE;
	public static int PLAYER_DROP_LIMIT;
	public static int PLAYER_RATE_DROP;
	public static int PLAYER_RATE_DROP_ITEM;
	public static int PLAYER_RATE_DROP_EQUIP;
	public static int PLAYER_RATE_DROP_EQUIP_WEAPON;
	public static float PET_XP_RATE;
	public static int PET_FOOD_RATE;
	public static float SINEATER_XP_RATE;
	public static int KARMA_DROP_LIMIT;
	public static int KARMA_RATE_DROP;
	public static int KARMA_RATE_DROP_ITEM;
	public static int KARMA_RATE_DROP_EQUIP;
	public static int KARMA_RATE_DROP_EQUIP_WEAPON;

	// --------------------------------------------------
	// Vitality Settings
	// --------------------------------------------------
	public static float RATE_VITALITY_EXP_MULTIPLIER;
	public static float RATE_LIMITED_SAYHA_GRACE_EXP_MULTIPLIER;
	public static int VITALITY_MAX_ITEMS_ALLOWED;
	public static float RATE_VITALITY_LOST;
	public static float RATE_VITALITY_GAIN;

	// --------------------------------------------------
	// No classification assigned to the following yet
	// --------------------------------------------------
	public static TimeSpan PVP_NORMAL_TIME;
	public static TimeSpan PVP_PVP_TIME;
	public static int MAX_REPUTATION;
	public static int REPUTATION_INCREASE;


	public static double HP_REGEN_MULTIPLIER;
	public static double MP_REGEN_MULTIPLIER;
	public static double CP_REGEN_MULTIPLIER;
	public static bool TRAINING_CAMP_ENABLE;
	public static bool TRAINING_CAMP_PREMIUM_ONLY;
	public static int TRAINING_CAMP_MAX_DURATION;
	public static int TRAINING_CAMP_MIN_LEVEL;
	public static int TRAINING_CAMP_MAX_LEVEL;
	public static double TRAINING_CAMP_EXP_MULTIPLIER;
	public static double TRAINING_CAMP_SP_MULTIPLIER;
	public static bool GAME_ASSISTANT_ENABLED;
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

	// Magic Lamp
	public static bool ENABLE_MAGIC_LAMP;
	public static int MAGIC_LAMP_MAX_LEVEL_EXP;
	public static double MAGIC_LAMP_CHARGE_RATE;

	// Random Craft
	public static bool ENABLE_RANDOM_CRAFT;
	public static int RANDOM_CRAFT_REFRESH_FEE;
	public static int RANDOM_CRAFT_CREATE_FEE;
	public static bool DROP_RANDOM_CRAFT_MATERIALS;

	// World Exchange
	public static bool ENABLE_WORLD_EXCHANGE;
	public static string WORLD_EXCHANGE_DEFAULT_LANG = "en";
	public static long WORLD_EXCHANGE_SAVE_INTERVAL;
	public static double WORLD_EXCHANGE_LCOIN_TAX;
	public static long WORLD_EXCHANGE_MAX_LCOIN_TAX;
	public static double WORLD_EXCHANGE_ADENA_FEE;
	public static long WORLD_EXCHANGE_MAX_ADENA_FEE;
	public static bool WORLD_EXCHANGE_LAZY_UPDATE;
	public static int WORLD_EXCHANGE_ITEM_SELL_PERIOD;
	public static int WORLD_EXCHANGE_ITEM_BACK_PERIOD;
	public static int WORLD_EXCHANGE_PAYMENT_TAKE_PERIOD;

	// HuntPass
	public static bool ENABLE_HUNT_PASS;
	public static int HUNT_PASS_PERIOD;
	public static int HUNT_PASS_PREMIUM_ITEM_ID;
	public static int HUNT_PASS_PREMIUM_ITEM_COUNT;
	public static int HUNT_PASS_POINTS_FOR_STEP;

	// Achivement Box
	public static bool ENABLE_ACHIEVEMENT_BOX;
	public static int ACHIEVEMENT_BOX_POINTS_FOR_REWARD;
	public static bool ENABLE_ACHIEVEMENT_PVP;
	public static int ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD;

	// GrandBoss Settings

	// Antharas
	public static int ANTHARAS_WAIT_TIME;
	public static int ANTHARAS_SPAWN_INTERVAL;
	public static int ANTHARAS_SPAWN_RANDOM;

	// Baium
	public static int BAIUM_SPAWN_INTERVAL;

	// Core
	public static int CORE_SPAWN_INTERVAL;
	public static int CORE_SPAWN_RANDOM;

	// Offen
	public static int ORFEN_SPAWN_INTERVAL;
	public static int ORFEN_SPAWN_RANDOM;

	// Queen Ant
	public static int QUEEN_ANT_SPAWN_INTERVAL;
	public static int QUEEN_ANT_SPAWN_RANDOM;

	// Zaken
	public static int ZAKEN_SPAWN_INTERVAL;
	public static int ZAKEN_SPAWN_RANDOM;

	// Balok
	public static TimeOnly BALOK_TIME;
	public static int BALOK_POINTS_PER_MONSTER;

	// Gracia Seeds Settings
	public static int SOD_TIAT_KILL_COUNT;
	public static long SOD_STAGE_2_LENGTH;

	// chatfilter
	public static ImmutableArray<string> FILTER_LIST = ImmutableArray<string>.Empty;

	// --------------------------------------------------
	// GeoEngine
	// --------------------------------------------------
	public static string GEODATA_PATH = string.Empty;
	public static string PATHNODE_PATH = string.Empty;
	public static string GEOEDIT_PATH = string.Empty;
	public static int PATHFINDING;
	public static string PATHFIND_BUFFERS = "100x6;128x6;192x6;256x4;320x4;384x4;500x2";
	public static float LOW_WEIGHT;
	public static float MEDIUM_WEIGHT;
	public static float HIGH_WEIGHT;
	public static bool ADVANCED_DIAGONAL_STRATEGY;
	public static float DIAGONAL_WEIGHT;
	public static int MAX_POSTFILTER_PASSES;
	public static bool DEBUG_PATH;

	// --------------------------------------------------
	// Custom Settings
	// --------------------------------------------------
	public static bool CHAMPION_ENABLE;
	public static bool CHAMPION_PASSIVE;
	public static int CHAMPION_FREQUENCY;
	public static string CHAMP_TITLE = "Champion";
	public static bool SHOW_CHAMPION_AURA;
	public static int CHAMP_MIN_LEVEL;
	public static int CHAMP_MAX_LEVEL;
	public static int CHAMPION_HP;
	public static float CHAMPION_REWARDS_EXP_SP;
	public static float CHAMPION_REWARDS_CHANCE;
	public static float CHAMPION_REWARDS_AMOUNT;
	public static float CHAMPION_ADENAS_REWARDS_CHANCE;
	public static float CHAMPION_ADENAS_REWARDS_AMOUNT;
	public static float CHAMPION_HP_REGEN;
	public static float CHAMPION_ATK;
	public static float CHAMPION_SPD_ATK;
	public static int CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE;
	public static int CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE;
	public static ImmutableDictionary<int, int> CHAMPION_REWARD_ITEMS = ImmutableDictionary<int, int>.Empty;
	public static bool CHAMPION_ENABLE_VITALITY;
	public static bool CHAMPION_ENABLE_IN_INSTANCES;
	public static bool BANKING_SYSTEM_ENABLED;
	public static int BANKING_SYSTEM_GOLDBARS;
	public static int BANKING_SYSTEM_ADENA;
	public static bool RAIDBOSS_SPAWN_ANNOUNCEMENTS;
	public static bool RAIDBOSS_DEFEAT_ANNOUNCEMENTS;
	public static bool RAIDBOSS_INSTANCE_ANNOUNCEMENTS;
	public static bool GRANDBOSS_SPAWN_ANNOUNCEMENTS;
	public static bool GRANDBOSS_DEFEAT_ANNOUNCEMENTS;
	public static bool GRANDBOSS_INSTANCE_ANNOUNCEMENTS;
	public static bool ENABLE_NPC_STAT_MULTIPLIERS;
	public static double MONSTER_HP_MULTIPLIER;
	public static double MONSTER_MP_MULTIPLIER;
	public static double MONSTER_PATK_MULTIPLIER;
	public static double MONSTER_MATK_MULTIPLIER;
	public static double MONSTER_PDEF_MULTIPLIER;
	public static double MONSTER_MDEF_MULTIPLIER;
	public static double MONSTER_AGRRO_RANGE_MULTIPLIER;
	public static double MONSTER_CLAN_HELP_RANGE_MULTIPLIER;
	public static double RAIDBOSS_HP_MULTIPLIER;
	public static double RAIDBOSS_MP_MULTIPLIER;
	public static double RAIDBOSS_PATK_MULTIPLIER;
	public static double RAIDBOSS_MATK_MULTIPLIER;
	public static double RAIDBOSS_PDEF_MULTIPLIER;
	public static double RAIDBOSS_MDEF_MULTIPLIER;
	public static double RAIDBOSS_AGRRO_RANGE_MULTIPLIER;
	public static double RAIDBOSS_CLAN_HELP_RANGE_MULTIPLIER;
	public static double GUARD_HP_MULTIPLIER;
	public static double GUARD_MP_MULTIPLIER;
	public static double GUARD_PATK_MULTIPLIER;
	public static double GUARD_MATK_MULTIPLIER;
	public static double GUARD_PDEF_MULTIPLIER;
	public static double GUARD_MDEF_MULTIPLIER;
	public static double GUARD_AGRRO_RANGE_MULTIPLIER;
	public static double GUARD_CLAN_HELP_RANGE_MULTIPLIER;
	public static double DEFENDER_HP_MULTIPLIER;
	public static double DEFENDER_MP_MULTIPLIER;
	public static double DEFENDER_PATK_MULTIPLIER;
	public static double DEFENDER_MATK_MULTIPLIER;
	public static double DEFENDER_PDEF_MULTIPLIER;
	public static double DEFENDER_MDEF_MULTIPLIER;
	public static double DEFENDER_AGRRO_RANGE_MULTIPLIER;
	public static double DEFENDER_CLAN_HELP_RANGE_MULTIPLIER;
	public static bool ENABLE_OFFLINE_PLAY_COMMAND;
	public static bool OFFLINE_PLAY_PREMIUM;
	public static bool OFFLINE_PLAY_LOGOUT_ON_DEATH;
	public static string OFFLINE_PLAY_LOGIN_MESSAGE = string.Empty;
	public static bool OFFLINE_PLAY_SET_NAME_COLOR;
	public static Color OFFLINE_PLAY_NAME_COLOR;

	public static ImmutableArray<AbnormalVisualEffect> OFFLINE_PLAY_ABNORMAL_EFFECTS =
		ImmutableArray<AbnormalVisualEffect>.Empty;

	public static bool OFFLINE_TRADE_ENABLE;
	public static bool OFFLINE_CRAFT_ENABLE;
	public static bool OFFLINE_MODE_IN_PEACE_ZONE;
	public static bool OFFLINE_MODE_NO_DAMAGE;
	public static bool RESTORE_OFFLINERS;
	public static int OFFLINE_MAX_DAYS;
	public static bool OFFLINE_DISCONNECT_FINISHED;
	public static bool OFFLINE_DISCONNECT_SAME_ACCOUNT;
	public static bool OFFLINE_SET_NAME_COLOR;
	public static Color OFFLINE_NAME_COLOR;

	public static ImmutableArray<AbnormalVisualEffect> OFFLINE_ABNORMAL_EFFECTS =
		ImmutableArray<AbnormalVisualEffect>.Empty;

	public static bool OFFLINE_FAME;
	public static bool STORE_OFFLINE_TRADE_IN_REALTIME;
	public static bool ENABLE_OFFLINE_COMMAND;
	public static bool DISPLAY_SERVER_TIME;
	public static int BUFFER_MAX_SCHEMES;
	public static int BUFFER_ITEM_ID;
	public static int BUFFER_STATIC_BUFF_COST;
	public static bool WELCOME_MESSAGE_ENABLED;
	public static string WELCOME_MESSAGE_TEXT = "Welcome to our server!";
	public static int WELCOME_MESSAGE_TIME;
	public static bool ANNOUNCE_PK_PVP;
	public static bool ANNOUNCE_PK_PVP_NORMAL_MESSAGE;
	public static string ANNOUNCE_PK_MSG = "$killer has slaughtered $target";
	public static string ANNOUNCE_PVP_MSG = "$killer has defeated $target";
	public static bool REWARD_PVP_ITEM;
	public static int REWARD_PVP_ITEM_ID;
	public static int REWARD_PVP_ITEM_AMOUNT;
	public static bool REWARD_PVP_ITEM_MESSAGE;
	public static bool REWARD_PK_ITEM;
	public static int REWARD_PK_ITEM_ID;
	public static int REWARD_PK_ITEM_AMOUNT;
	public static bool REWARD_PK_ITEM_MESSAGE;
	public static bool DISABLE_REWARDS_IN_INSTANCES;
	public static bool DISABLE_REWARDS_IN_PVP_ZONES;
	public static bool PVP_COLOR_SYSTEM_ENABLED;
	public static int PVP_AMOUNT1;
	public static int PVP_AMOUNT2;
	public static int PVP_AMOUNT3;
	public static int PVP_AMOUNT4;
	public static int PVP_AMOUNT5;
	public static Color NAME_COLOR_FOR_PVP_AMOUNT1;
	public static Color NAME_COLOR_FOR_PVP_AMOUNT2;
	public static Color NAME_COLOR_FOR_PVP_AMOUNT3;
	public static Color NAME_COLOR_FOR_PVP_AMOUNT4;
	public static Color NAME_COLOR_FOR_PVP_AMOUNT5;
	public static string TITLE_FOR_PVP_AMOUNT1 = "Title";
	public static string TITLE_FOR_PVP_AMOUNT2 = "Title";
	public static string TITLE_FOR_PVP_AMOUNT3 = "Title";
	public static string TITLE_FOR_PVP_AMOUNT4 = "Title";
	public static string TITLE_FOR_PVP_AMOUNT5 = "Title";
	public static bool CHAT_ADMIN;

	// TODO: change all multiplier's types from dictionary to array for fast lookup
	public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_BLOW_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_BLOW_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_BLOW_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_BLOW_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_ENERGY_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_ENERGY_SKILL_DAMAGE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVE_ENERGY_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PVP_ENERGY_SKILL_DEFENCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> PLAYER_HEALING_SKILL_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> SKILL_MASTERY_CHANCE_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> EXP_AMOUNT_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static ImmutableDictionary<CharacterClass, double> SP_AMOUNT_MULTIPLIERS =
		ImmutableDictionary<CharacterClass, double>.Empty;

	public static bool MULTILANG_ENABLE;
	public static ImmutableArray<string> MULTILANG_ALLOWED = ImmutableArray<string>.Empty;
	public static string MULTILANG_DEFAULT = "en";
	public static bool MULTILANG_VOICED_ALLOW;
	public static bool NOBLESS_MASTER_ENABLED;
	public static int NOBLESS_MASTER_NPCID;
	public static int NOBLESS_MASTER_LEVEL_REQUIREMENT;
	public static bool NOBLESS_MASTER_REWARD_TIARA;
	public static bool L2WALKER_PROTECTION;
	public static int DUALBOX_CHECK_MAX_PLAYERS_PER_IP;
	public static int DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP;
	public static int DUALBOX_CHECK_MAX_L2EVENT_PARTICIPANTS_PER_IP;
	public static bool DUALBOX_COUNT_OFFLINE_TRADERS;
	public static FrozenDictionary<int, int> DUALBOX_CHECK_WHITELIST = FrozenDictionary<int, int>.Empty;
	public static bool ENABLE_ONLINE_COMMAND;
	public static bool ALLOW_CHANGE_PASSWORD;
	public static bool ALLOW_HUMAN;
	public static bool ALLOW_ELF;
	public static bool ALLOW_DARKELF;
	public static bool ALLOW_ORC;
	public static bool ALLOW_DWARF;
	public static bool ALLOW_KAMAEL;
	public static bool ALLOW_DEATH_KNIGHT;
	public static bool ALLOW_SYLPH;
	public static bool ALLOW_VANGUARD;
	public static bool AUTO_POTIONS_ENABLED;
	public static bool AUTO_POTIONS_IN_OLYMPIAD;
	public static int AUTO_POTION_MIN_LEVEL;
	public static bool AUTO_CP_ENABLED;
	public static bool AUTO_HP_ENABLED;
	public static bool AUTO_MP_ENABLED;
	public static int AUTO_CP_PERCENTAGE;
	public static int AUTO_HP_PERCENTAGE;
	public static int AUTO_MP_PERCENTAGE;
	public static ImmutableSortedSet<int> AUTO_CP_ITEM_IDS = ImmutableSortedSet<int>.Empty;
	public static ImmutableSortedSet<int> AUTO_HP_ITEM_IDS = ImmutableSortedSet<int>.Empty;
	public static ImmutableSortedSet<int> AUTO_MP_ITEM_IDS = ImmutableSortedSet<int>.Empty;
	public static bool CUSTOM_STARTING_LOC;
	public static int CUSTOM_STARTING_LOC_X;
	public static int CUSTOM_STARTING_LOC_Y;
	public static int CUSTOM_STARTING_LOC_Z;
	public static int SHOP_MIN_RANGE_FROM_NPC;
	public static int SHOP_MIN_RANGE_FROM_PLAYER;
	public static bool ENABLE_RANDOM_MONSTER_SPAWNS;
	public static int MOB_MIN_SPAWN_RANGE;
	public static int MOB_MAX_SPAWN_RANGE;
	public static ImmutableSortedSet<int> MOBS_LIST_NOT_RANDOM = ImmutableSortedSet<int>.Empty;
	public static bool FREE_JUMPS_FOR_ALL;
	public static bool CUSTOM_CB_ENABLED;
	public static int COMMUNITYBOARD_CURRENCY;
	public static bool COMMUNITYBOARD_ENABLE_MULTISELLS;
	public static bool COMMUNITYBOARD_ENABLE_TELEPORTS;
	public static bool COMMUNITYBOARD_ENABLE_BUFFS;
	public static bool COMMUNITYBOARD_ENABLE_HEAL;
	public static bool COMMUNITYBOARD_ENABLE_DELEVEL;
	public static int COMMUNITYBOARD_TELEPORT_PRICE;
	public static int COMMUNITYBOARD_BUFF_PRICE;
	public static int COMMUNITYBOARD_HEAL_PRICE;
	public static int COMMUNITYBOARD_DELEVEL_PRICE;
	public static bool COMMUNITYBOARD_COMBAT_DISABLED;
	public static bool COMMUNITYBOARD_KARMA_DISABLED;
	public static bool COMMUNITYBOARD_CAST_ANIMATIONS;
	public static bool COMMUNITY_PREMIUM_SYSTEM_ENABLED;
	public static int COMMUNITY_PREMIUM_COIN_ID;
	public static int COMMUNITY_PREMIUM_PRICE_PER_DAY;
	public static ImmutableSortedSet<int> COMMUNITY_AVAILABLE_BUFFS = ImmutableSortedSet<int>.Empty;

	public static ImmutableDictionary<string, Location> COMMUNITY_AVAILABLE_TELEPORTS =
		ImmutableDictionary<string, Location>.Empty;

	public static bool CUSTOM_DEPOSITABLE_ENABLED;
	public static bool CUSTOM_DEPOSITABLE_QUEST_ITEMS;
	public static bool CUSTOM_MAIL_MANAGER_ENABLED;
	public static int CUSTOM_MAIL_MANAGER_DELAY;
	public static bool DELEVEL_MANAGER_ENABLED;
	public static int DELEVEL_MANAGER_NPCID;
	public static int DELEVEL_MANAGER_ITEMID;
	public static int DELEVEL_MANAGER_ITEMCOUNT;
	public static int DELEVEL_MANAGER_MINIMUM_DELEVEL;
	public static bool FACTION_SYSTEM_ENABLED;
	public static Location FACTION_STARTING_LOCATION;
	public static Location FACTION_MANAGER_LOCATION;
	public static Location FACTION_GOOD_BASE_LOCATION;
	public static Location FACTION_EVIL_BASE_LOCATION;
	public static string FACTION_GOOD_TEAM_NAME = "Good";
	public static string FACTION_EVIL_TEAM_NAME = "Evil";
	public static Color FACTION_GOOD_NAME_COLOR;
	public static Color FACTION_EVIL_NAME_COLOR;
	public static bool FACTION_GUARDS_ENABLED;
	public static bool FACTION_RESPAWN_AT_BASE;
	public static bool FACTION_AUTO_NOBLESS;
	public static bool FACTION_SPECIFIC_CHAT;
	public static bool FACTION_BALANCE_ONLINE_PLAYERS;
	public static int FACTION_BALANCE_PLAYER_EXCEED_LIMIT;
	public static bool FAKE_PLAYERS_ENABLED;
	public static bool FAKE_PLAYER_CHAT;
	public static bool FAKE_PLAYER_USE_SHOTS;
	public static bool FAKE_PLAYER_KILL_PVP;
	public static bool FAKE_PLAYER_KILL_KARMA;
	public static bool FAKE_PLAYER_AUTO_ATTACKABLE;
	public static bool FAKE_PLAYER_AGGRO_MONSTERS;
	public static bool FAKE_PLAYER_AGGRO_PLAYERS;
	public static bool FAKE_PLAYER_AGGRO_FPC;
	public static bool FAKE_PLAYER_CAN_DROP_ITEMS;
	public static bool FAKE_PLAYER_CAN_PICKUP;
	public static bool ENABLE_FIND_PVP;
	public static bool MERCHANT_ZERO_SELL_PRICE;
	public static bool PREMIUM_SYSTEM_ENABLED;
	public static float PREMIUM_RATE_XP;

	public static float PREMIUM_RATE_SP;

	//public static ImmutableDictionary<int, double> PREMIUM_RATE_DROP_ITEMS_ID = ImmutableDictionary<int, double>.Empty;
	public static float PREMIUM_RATE_DROP_CHANCE;
	public static float PREMIUM_RATE_DROP_AMOUNT;
	public static float PREMIUM_RATE_SPOIL_CHANCE;
	public static float PREMIUM_RATE_SPOIL_AMOUNT;
	public static float PREMIUM_RATE_QUEST_XP;
	public static float PREMIUM_RATE_QUEST_SP;

	public static ImmutableDictionary<int, double> PREMIUM_RATE_DROP_CHANCE_BY_ID =
		ImmutableDictionary<int, double>.Empty;

	public static ImmutableDictionary<int, double> PREMIUM_RATE_DROP_AMOUNT_BY_ID =
		ImmutableDictionary<int, double>.Empty;

	public static bool PREMIUM_ONLY_FISHING;
	public static bool PC_CAFE_ENABLED;
	public static bool PC_CAFE_ONLY_PREMIUM;
	public static bool PC_CAFE_ONLY_VIP;
	public static bool PC_CAFE_RETAIL_LIKE;
	public static int PC_CAFE_REWARD_TIME;
	public static int PC_CAFE_MAX_POINTS;
	public static bool PC_CAFE_ENABLE_DOUBLE_POINTS;
	public static int PC_CAFE_DOUBLE_POINTS_CHANCE;
	public static int ACQUISITION_PC_CAFE_RETAIL_LIKE_POINTS;
	public static double PC_CAFE_POINT_RATE;
	public static bool PC_CAFE_RANDOM_POINT;
	public static bool PC_CAFE_REWARD_LOW_EXP_KILLS;
	public static int PC_CAFE_LOW_EXP_KILLS_CHANCE;
	public static bool VIP_SYSTEM_ENABLED;
	public static bool VIP_SYSTEM_PRIME_AFFECT;
	public static bool VIP_SYSTEM_L_SHOP_AFFECT;
	public static int VIP_SYSTEM_MAX_TIER;
	public static bool SELLBUFF_ENABLED;
	public static int SELLBUFF_MP_MULTIPLER;
	public static int SELLBUFF_PAYMENT_ID;
	public static long SELLBUFF_MIN_PRICE;
	public static long SELLBUFF_MAX_PRICE;
	public static int SELLBUFF_MAX_BUFFS;

	public static string SUBJUGATION_TOPIC_HEADER = "Purge reward";

    public static string SUBJUGATION_TOPIC_BODY =
        "Reward for being in the top of the best players in clearing the lands of Aden";

	internal static void Load(string configPath, string dataPackPath)
    {
        Server.Load(configPath);
        Feature.Load(configPath);
        Attendance.Load(configPath);
        AttributeSystem.Load(configPath);
        Character.Load(configPath);

        ConfigurationParser parser = new(configPath);

		// Load Magic Lamp config file (if exists)
		parser.LoadConfig(MAGIC_LAMP_FILE);
		ENABLE_MAGIC_LAMP = parser.getBoolean("MagicLampEnabled");
		MAGIC_LAMP_MAX_LEVEL_EXP = parser.getInt("MagicLampMaxLevelExp", 10000000);
		MAGIC_LAMP_CHARGE_RATE = parser.getDouble("MagicLampChargeRate", 0.1);

		// Load Random Craft config file (if exists)
		parser.LoadConfig(RANDOM_CRAFT_FILE);
		ENABLE_RANDOM_CRAFT = parser.getBoolean("RandomCraftEnabled", true);
		RANDOM_CRAFT_REFRESH_FEE = parser.getInt("RandomCraftRefreshFee", 50000);
		RANDOM_CRAFT_CREATE_FEE = parser.getInt("RandomCraftCreateFee", 300000);
		DROP_RANDOM_CRAFT_MATERIALS = parser.getBoolean("DropRandomCraftMaterials", true);

		// Load World Exchange config file (if exists)
		parser.LoadConfig(WORLD_EXCHANGE_FILE);
		ENABLE_WORLD_EXCHANGE = parser.getBoolean("EnableWorldExchange", true);
		WORLD_EXCHANGE_DEFAULT_LANG = parser.getString("WorldExchangeDefaultLanguage", "en");
		WORLD_EXCHANGE_SAVE_INTERVAL = parser.getLong("BidItemsIntervalStatusCheck", 30000);
		WORLD_EXCHANGE_LCOIN_TAX = parser.getDouble("LCoinFee", 0.05);
		WORLD_EXCHANGE_MAX_LCOIN_TAX = parser.getLong("MaxLCoinFee", 20000);
		WORLD_EXCHANGE_ADENA_FEE = parser.getDouble("AdenaFee", 100.0);
		WORLD_EXCHANGE_MAX_ADENA_FEE = parser.getLong("MaxAdenaFee", -1);
		WORLD_EXCHANGE_LAZY_UPDATE = parser.getBoolean("DBLazy");
		WORLD_EXCHANGE_ITEM_SELL_PERIOD = parser.getInt("ItemSellPeriod", 14);
		WORLD_EXCHANGE_ITEM_BACK_PERIOD = parser.getInt("ItemBackPeriod", 120);
		WORLD_EXCHANGE_PAYMENT_TAKE_PERIOD = parser.getInt("PaymentTakePeriod", 120);

		// Load Training Camp config file (if exists)
		parser.LoadConfig(TRAINING_CAMP_CONFIG_FILE);
		TRAINING_CAMP_ENABLE = parser.getBoolean("TrainingCampEnable");
		TRAINING_CAMP_PREMIUM_ONLY = parser.getBoolean("TrainingCampPremiumOnly");
		TRAINING_CAMP_MAX_DURATION = parser.getInt("TrainingCampDuration", 18000);
		TRAINING_CAMP_MIN_LEVEL = parser.getInt("TrainingCampMinLevel", 18);
		TRAINING_CAMP_MAX_LEVEL = parser.getInt("TrainingCampMaxLevel", 127);
		TRAINING_CAMP_EXP_MULTIPLIER = parser.getDouble("TrainingCampExpMultiplier", 1.0);
		TRAINING_CAMP_SP_MULTIPLIER = parser.getDouble("TrainingCampSpMultiplier", 1.0);

		// Load GameAssistant config file (if exists)
		parser.LoadConfig(GAME_ASSISTANT_CONFIG_FILE);
		GAME_ASSISTANT_ENABLED = parser.getBoolean("GameAssistantEnabled");

		// Load General config file (if exists)
		parser.LoadConfig(GENERAL_CONFIG_FILE);
		DEFAULT_ACCESS_LEVEL = parser.getInt("DefaultAccessLevel");
		SERVER_GMONLY = parser.getBoolean("ServerGMOnly");
		GM_HERO_AURA = parser.getBoolean("GMHeroAura");
		GM_STARTUP_BUILDER_HIDE = parser.getBoolean("GMStartupBuilderHide");
		GM_STARTUP_INVULNERABLE = parser.getBoolean("GMStartupInvulnerable");
		GM_STARTUP_INVISIBLE = parser.getBoolean("GMStartupInvisible");
		GM_STARTUP_SILENCE = parser.getBoolean("GMStartupSilence");
		GM_STARTUP_AUTO_LIST = parser.getBoolean("GMStartupAutoList");
		GM_STARTUP_DIET_MODE = parser.getBoolean("GMStartupDietMode");
		GM_ITEM_RESTRICTION = parser.getBoolean("GMItemRestriction", true);
		GM_SKILL_RESTRICTION = parser.getBoolean("GMSkillRestriction", true);
		GM_TRADE_RESTRICTED_ITEMS = parser.getBoolean("GMTradeRestrictedItems");
		GM_RESTART_FIGHTING = parser.getBoolean("GMRestartFighting", true);
		GM_ANNOUNCER_NAME = parser.getBoolean("GMShowAnnouncerName");
		GM_GIVE_SPECIAL_SKILLS = parser.getBoolean("GMGiveSpecialSkills");
		GM_GIVE_SPECIAL_AURA_SKILLS = parser.getBoolean("GMGiveSpecialAuraSkills");
		GM_DEBUG_HTML_PATHS = parser.getBoolean("GMDebugHtmlPaths", true);
		USE_SUPER_HASTE_AS_GM_SPEED = parser.getBoolean("UseSuperHasteAsGMSpeed");
		LOG_CHAT = parser.getBoolean("LogChat");
		LOG_AUTO_ANNOUNCEMENTS = parser.getBoolean("LogAutoAnnouncements");
		LOG_ITEMS = parser.getBoolean("LogItems");
		LOG_ITEMS_SMALL_LOG = parser.getBoolean("LogItemsSmallLog");
		LOG_ITEMS_IDS_ONLY = parser.getBoolean("LogItemsIdsOnly");
		LOG_ITEMS_IDS_LIST = parser.GetIntList("LogItemsIdsList").ToImmutableSortedSet();
		LOG_ITEM_ENCHANTS = parser.getBoolean("LogItemEnchants");
		LOG_SKILL_ENCHANTS = parser.getBoolean("LogSkillEnchants");
		GMAUDIT = parser.getBoolean("GMAudit");
		SKILL_CHECK_ENABLE = parser.getBoolean("SkillCheckEnable");
		SKILL_CHECK_REMOVE = parser.getBoolean("SkillCheckRemove");
		SKILL_CHECK_GM = parser.getBoolean("SkillCheckGM", true);
		HTML_ACTION_CACHE_DEBUG = parser.getBoolean("HtmlActionCacheDebug");
		DEVELOPER = parser.getBoolean("Developer");
		ALT_DEV_NO_QUESTS = parser.getBoolean("AltDevNoQuests");
		ALT_DEV_NO_SPAWNS = parser.getBoolean("AltDevNoSpawns");
		ALT_DEV_SHOW_QUESTS_LOAD_IN_LOGS = parser.getBoolean("AltDevShowQuestsLoadInLogs");
		ALT_DEV_SHOW_SCRIPTS_LOAD_IN_LOGS = parser.getBoolean("AltDevShowScriptsLoadInLogs");
		DEBUG_CLIENT_PACKETS = parser.getBoolean("DebugClientPackets");
		DEBUG_EX_CLIENT_PACKETS = parser.getBoolean("DebugExClientPackets");
		DEBUG_SERVER_PACKETS = parser.getBoolean("DebugServerPackets");
		DEBUG_UNKNOWN_PACKETS = parser.getBoolean("DebugUnknownPackets", true);
		ALT_DEV_EXCLUDED_PACKETS = parser.GetStringList("ExcludedPacketList")
			.ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);
		ALLOW_DISCARDITEM = parser.getBoolean("AllowDiscardItem", true);
		AUTODESTROY_ITEM_AFTER = parser.getInt("AutoDestroyDroppedItemAfter", 600);
		HERB_AUTO_DESTROY_TIME = parser.getInt("AutoDestroyHerbTime", 60) * 1000;
		LIST_PROTECTED_ITEMS = parser.GetIntList("ListOfProtectedItems").ToImmutableSortedSet();
		DATABASE_CLEAN_UP = parser.getBoolean("DatabaseCleanUp", true);
		CHAR_DATA_STORE_INTERVAL = parser.getInt("CharacterDataStoreInterval", 15) * 60 * 1000;
		CLAN_VARIABLES_STORE_INTERVAL = parser.getInt("ClanVariablesStoreInterval", 15) * 60 * 1000;
		LAZY_ITEMS_UPDATE = parser.getBoolean("LazyItemsUpdate");
		UPDATE_ITEMS_ON_CHAR_STORE = parser.getBoolean("UpdateItemsOnCharStore");
		DESTROY_DROPPED_PLAYER_ITEM = parser.getBoolean("DestroyPlayerDroppedItem");
		DESTROY_EQUIPABLE_PLAYER_ITEM = parser.getBoolean("DestroyEquipableItem");
		DESTROY_ALL_ITEMS = parser.getBoolean("DestroyAllItems");
		SAVE_DROPPED_ITEM = parser.getBoolean("SaveDroppedItem");
		EMPTY_DROPPED_ITEM_TABLE_AFTER_LOAD = parser.getBoolean("EmptyDroppedItemTableAfterLoad");
		SAVE_DROPPED_ITEM_INTERVAL = parser.getInt("SaveDroppedItemInterval", 60) * 60000;
		CLEAR_DROPPED_ITEM_TABLE = parser.getBoolean("ClearDroppedItemTable");
		ORDER_QUEST_LIST_BY_QUESTID = parser.getBoolean("OrderQuestListByQuestId", true);
		AUTODELETE_INVALID_QUEST_DATA = parser.getBoolean("AutoDeleteInvalidQuestData");
		ENABLE_STORY_QUEST_BUFF_REWARD = parser.getBoolean("StoryQuestRewardBuff", true);
		MULTIPLE_ITEM_DROP = parser.getBoolean("MultipleItemDrop", true);
		HTM_CACHE = parser.getBoolean("HtmCache", true);
		CHECK_HTML_ENCODING = parser.getBoolean("CheckHtmlEncoding", true);
		HIDE_BYPASS_REMOVAL = parser.getBoolean("HideBypassRemoval", true);
		MIN_NPC_ANIMATION = parser.getInt("MinNpcAnimation", 5);
		MAX_NPC_ANIMATION = parser.getInt("MaxNpcAnimation", 60);
		MIN_MONSTER_ANIMATION = parser.getInt("MinMonsterAnimation", 5);
		MAX_MONSTER_ANIMATION = parser.getInt("MaxMonsterAnimation", 60);
		GRIDS_ALWAYS_ON = parser.getBoolean("GridsAlwaysOn");
		GRID_NEIGHBOR_TURNON_TIME = parser.getInt("GridNeighborTurnOnTime", 1);
		GRID_NEIGHBOR_TURNOFF_TIME = parser.getInt("GridNeighborTurnOffTime", 90);
		CORRECT_PRICES = parser.getBoolean("CorrectPrices", true);
		ENABLE_FALLING_DAMAGE = parser.getBoolean("EnableFallingDamage", true);
		PEACE_ZONE_MODE = parser.getInt("PeaceZoneMode");
		DEFAULT_GLOBAL_CHAT = parser.getString("GlobalChat", "ON");
		DEFAULT_TRADE_CHAT = parser.getString("TradeChat", "ON");
		ENABLE_WORLD_CHAT = parser.getBoolean("WorldChatEnabled", true);
		MINIMUM_CHAT_LEVEL = parser.getInt("MinimumChatLevel", 20);
		ALLOW_WAREHOUSE = parser.getBoolean("AllowWarehouse", true);
		ALLOW_REFUND = parser.getBoolean("AllowRefund", true);
		ALLOW_MAIL = parser.getBoolean("AllowMail", true);
		ALLOW_ATTACHMENTS = parser.getBoolean("AllowAttachments", true);
		ALLOW_WEAR = parser.getBoolean("AllowWear", true);
		WEAR_DELAY = parser.getInt("WearDelay", 5);
		WEAR_PRICE = parser.getInt("WearPrice", 10);
		STORE_REVIEW_LIMIT = parser.getInt("PrivateStoreReviewLimit", 30);
		STORE_REVIEW_CACHE_TIME = parser.getInt("PrivateStoreReviewCacheTime", 5000);
		INSTANCE_FINISH_TIME = parser.getInt("DefaultFinishTime", 5);
		RESTORE_PLAYER_INSTANCE = parser.getBoolean("RestorePlayerInstance");
		EJECT_DEAD_PLAYER_TIME = parser.getInt("EjectDeadPlayerTime", 1);
		ALLOW_RACE = parser.getBoolean("AllowRace", true);
		ALLOW_WATER = parser.getBoolean("AllowWater", true);
		ALLOW_FISHING = parser.getBoolean("AllowFishing", true);
		ALLOW_MANOR = parser.getBoolean("AllowManor", true);
		ALLOW_BOAT = parser.getBoolean("AllowBoat", true);
		BOAT_BROADCAST_RADIUS = parser.getInt("BoatBroadcastRadius", 20000);
		ALLOW_CURSED_WEAPONS = parser.getBoolean("AllowCursedWeapons", true);
		SERVER_NEWS = parser.getBoolean("ShowServerNews");
		ENABLE_COMMUNITY_BOARD = parser.getBoolean("EnableCommunityBoard", true);
		BBS_DEFAULT = parser.getString("BBSDefault", "_bbshome");
		USE_SAY_FILTER = parser.getBoolean("UseChatFilter");
		CHAT_FILTER_CHARS = parser.getString("ChatFilterChars", "^_^");
		BAN_CHAT_CHANNELS = parser.GetEnumList("BanChatChannels", ';', ChatType.GENERAL, ChatType.SHOUT,
			ChatType.WORLD, ChatType.TRADE, ChatType.HERO_VOICE).ToImmutableSortedSet();

		WORLD_CHAT_MIN_LEVEL = parser.getInt("WorldChatMinLevel", 95);
		WORLD_CHAT_POINTS_PER_DAY = parser.getInt("WorldChatPointsPerDay", 10);
		WORLD_CHAT_INTERVAL = parser.GetTimeSpan("WorldChatInterval", TimeSpan.FromSeconds(20));
		ALT_MANOR_REFRESH_TIME = parser.getInt("AltManorRefreshTime", 20);
		ALT_MANOR_REFRESH_MIN = parser.getInt("AltManorRefreshMin");
		ALT_MANOR_APPROVE_TIME = parser.getInt("AltManorApproveTime", 4);
		ALT_MANOR_APPROVE_MIN = parser.getInt("AltManorApproveMin", 30);
		ALT_MANOR_MAINTENANCE_MIN = parser.getInt("AltManorMaintenanceMin", 6);
		ALT_MANOR_SAVE_ALL_ACTIONS = parser.getBoolean("AltManorSaveAllActions");
		ALT_MANOR_SAVE_PERIOD_RATE = parser.getInt("AltManorSavePeriodRate", 2);
		ALT_ITEM_AUCTION_ENABLED = parser.getBoolean("AltItemAuctionEnabled", true);
		ALT_ITEM_AUCTION_EXPIRED_AFTER = parser.getInt("AltItemAuctionExpiredAfter", 14);
		ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID = parser.getInt("AltItemAuctionTimeExtendsOnBid") * 1000;
		DEFAULT_PUNISH = parser.GetEnum("DefaultPunish", IllegalActionPunishmentType.KICK);
		DEFAULT_PUNISH_PARAM = parser.getLong("DefaultPunishParam");
		if (DEFAULT_PUNISH_PARAM == 0)
		{
			DEFAULT_PUNISH_PARAM = 3155695200L; // One hundred years in seconds.
		}

		ONLY_GM_ITEMS_FREE = parser.getBoolean("OnlyGMItemsFree", true);
		JAIL_IS_PVP = parser.getBoolean("JailIsPvp");
		JAIL_DISABLE_CHAT = parser.getBoolean("JailDisableChat", true);
		JAIL_DISABLE_TRANSACTION = parser.getBoolean("JailDisableTransaction");
		CUSTOM_NPC_DATA = parser.getBoolean("CustomNpcData");
		CUSTOM_TELEPORT_TABLE = parser.getBoolean("CustomTeleportTable");
		CUSTOM_SKILLS_LOAD = parser.getBoolean("CustomSkillsLoad");
		CUSTOM_ITEMS_LOAD = parser.getBoolean("CustomItemsLoad");
		CUSTOM_MULTISELL_LOAD = parser.getBoolean("CustomMultisellLoad");
		CUSTOM_BUYLIST_LOAD = parser.getBoolean("CustomBuyListLoad");
		BOOKMARK_CONSUME_ITEM_ID = parser.getInt("BookmarkConsumeItemId", -1);
		ALT_BIRTHDAY_GIFT = parser.getInt("AltBirthdayGift", 72078);
		ALT_BIRTHDAY_MAIL_SUBJECT = parser.getString("AltBirthdayMailSubject", "Happy Birthday!");
		ALT_BIRTHDAY_MAIL_TEXT = parser.getString("AltBirthdayMailText",
			"Hello Adventurer!! Seeing as you're one year older now, I thought I would send you some birthday cheer :) Please find your birthday pack attached. May these gifts bring you joy and happiness on this very special day." +
			Environment.NewLine + Environment.NewLine + "Sincerely, Alegria");
		BOTREPORT_ENABLE = parser.getBoolean("EnableBotReportButton");
		BOTREPORT_RESETPOINT_HOUR = parser.GetTime("BotReportPointsResetHour");
		BOTREPORT_REPORT_DELAY = parser.getInt("BotReportDelay", 30) * 60000;
		BOTREPORT_ALLOW_REPORTS_FROM_SAME_CLAN_MEMBERS = parser.getBoolean("AllowReportsFromSameClanMembers");
		ENABLE_AUTO_PLAY = parser.getBoolean("EnableAutoPlay", true);
		ENABLE_AUTO_POTION = parser.getBoolean("EnableAutoPotion", true);
		ENABLE_AUTO_PET_POTION = parser.getBoolean("EnableAutoPetPotion", true);
		ENABLE_AUTO_SKILL = parser.getBoolean("EnableAutoSkill", true);
		ENABLE_AUTO_ITEM = parser.getBoolean("EnableAutoItem", true);
		AUTO_PLAY_ATTACK_ACTION = parser.getBoolean("AutoPlayAttackAction", true);
		RESUME_AUTO_PLAY = parser.getBoolean("ResumeAutoPlay");
		ENABLE_AUTO_ASSIST = parser.getBoolean("AssistLeader");
		BLUE_TEAM_ABNORMAL_EFFECT = parser.GetEnum("BlueTeamAbnormalEffect", AbnormalVisualEffect.None);
		RED_TEAM_ABNORMAL_EFFECT = parser.GetEnum("RedTeamAbnormalEffect", AbnormalVisualEffect.None);
		SUBJUGATION_TOPIC_BODY = parser.getString("SubjugationTopicBody",
			"Reward for being in the top of the best players in clearing the lands of Aden");
		SUBJUGATION_TOPIC_HEADER = parser.getString("SubjugationTopicHeader", "Purge reward");
		SHARING_LOCATION_COST = parser.getInt("ShareLocationLcoinCost", 50);
		TELEPORT_SHARE_LOCATION_COST = parser.getInt("TeleportShareLocationLcoinCost", 400);

		// Load FloodProtector config file
		parser.LoadConfig(FLOOD_PROTECTOR_CONFIG_FILE);
		LoadFloodProtectorConfigs(parser);

		// Load NPC config file (if exists)
		parser.LoadConfig(NPC_CONFIG_FILE);
		ANNOUNCE_MAMMON_SPAWN = parser.getBoolean("AnnounceMammonSpawn");
		ALT_MOB_AGRO_IN_PEACEZONE = parser.getBoolean("AltMobAgroInPeaceZone", true);
		ALT_ATTACKABLE_NPCS = parser.getBoolean("AltAttackableNpcs", true);
		ALT_GAME_VIEWNPC = parser.getBoolean("AltGameViewNpc");
		SHOW_NPC_LEVEL = parser.getBoolean("ShowNpcLevel");
		SHOW_NPC_AGGRESSION = parser.getBoolean("ShowNpcAggression");
		ATTACKABLES_CAMP_PLAYER_CORPSES = parser.getBoolean("AttackablesCampPlayerCorpses");
		SHOW_CREST_WITHOUT_QUEST = parser.getBoolean("ShowCrestWithoutQuest");
		ENABLE_RANDOM_ENCHANT_EFFECT = parser.getBoolean("EnableRandomEnchantEffect");
		MIN_NPC_LEVEL_DMG_PENALTY = parser.getInt("MinNPCLevelForDmgPenalty", 78);
		NPC_DMG_PENALTY = parser.GetDoubleList("DmgPenaltyForLvLDifferences", ',', 0.7, 0.6, 0.6, 0.55);
		NPC_CRIT_DMG_PENALTY = parser.GetDoubleList("CritDmgPenaltyForLvLDifferences", ',', 0.75, 0.65, 0.6, 0.58);
		NPC_SKILL_DMG_PENALTY = parser.GetDoubleList("SkillDmgPenaltyForLvLDifferences", ',', 0.8, 0.7, 0.65, 0.62);
		MIN_NPC_LEVEL_MAGIC_PENALTY = parser.getInt("MinNPCLevelForMagicPenalty", 78);
		NPC_SKILL_CHANCE_PENALTY = parser.GetDoubleList("SkillChancePenaltyForLvLDifferences", ',', 2.5, 3.0, 3.25, 3.5);
		DEFAULT_CORPSE_TIME = parser.getInt("DefaultCorpseTime", 7);
		SPOILED_CORPSE_EXTEND_TIME = parser.getInt("SpoiledCorpseExtendTime", 10);
		CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY = TimeSpan.FromMilliseconds(
			parser.getInt("CorpseConsumeSkillAllowedTimeBeforeDecay", 2000));
		MAX_DRIFT_RANGE = parser.getInt("MaxDriftRange", 300);
		AGGRO_DISTANCE_CHECK_ENABLED = parser.getBoolean("AggroDistanceCheckEnabled", true);
		AGGRO_DISTANCE_CHECK_RANGE = parser.getInt("AggroDistanceCheckRange", 1500);
		AGGRO_DISTANCE_CHECK_RAIDS = parser.getBoolean("AggroDistanceCheckRaids");
		AGGRO_DISTANCE_CHECK_RAID_RANGE = parser.getInt("AggroDistanceCheckRaidRange", 3000);
		AGGRO_DISTANCE_CHECK_INSTANCES = parser.getBoolean("AggroDistanceCheckInstances");
		AGGRO_DISTANCE_CHECK_RESTORE_LIFE = parser.getBoolean("AggroDistanceCheckRestoreLife", true);
		GUARD_ATTACK_AGGRO_MOB = parser.getBoolean("GuardAttackAggroMob");
		RAID_HP_REGEN_MULTIPLIER = parser.getDouble("RaidHpRegenMultiplier", 100.0) / 100.0;
		RAID_MP_REGEN_MULTIPLIER = parser.getDouble("RaidMpRegenMultiplier", 100.0) / 100.0;
		RAID_PDEFENCE_MULTIPLIER = parser.getDouble("RaidPDefenceMultiplier", 100.0) / 100.0;
		RAID_MDEFENCE_MULTIPLIER = parser.getDouble("RaidMDefenceMultiplier", 100.0) / 100.0;
		RAID_PATTACK_MULTIPLIER = parser.getDouble("RaidPAttackMultiplier", 100.0) / 100.0;
		RAID_MATTACK_MULTIPLIER = parser.getDouble("RaidMAttackMultiplier", 100.0) / 100.0;
		RAID_MIN_RESPAWN_MULTIPLIER = parser.getFloat("RaidMinRespawnMultiplier", 1.0f);
		RAID_MAX_RESPAWN_MULTIPLIER = parser.getFloat("RaidMaxRespawnMultiplier", 1.0f);
		RAID_MINION_RESPAWN_TIMER = parser.getInt("RaidMinionRespawnTime", 300000);
		MINIONS_RESPAWN_TIME = parser.GetIdValueMap<int>("CustomMinionsRespawnTime");
		FORCE_DELETE_MINIONS = parser.getBoolean("ForceDeleteMinions");
		RAID_DISABLE_CURSE = parser.getBoolean("DisableRaidCurse");
		RAID_CHAOS_TIME = parser.getInt("RaidChaosTime", 10);
		GRAND_CHAOS_TIME = parser.getInt("GrandChaosTime", 10);
		MINION_CHAOS_TIME = parser.getInt("MinionChaosTime", 10);
		INVENTORY_MAXIMUM_PET = parser.getInt("MaximumSlotsForPet", 12);
		PET_HP_REGEN_MULTIPLIER = parser.getDouble("PetHpRegenMultiplier", 100) / 100;
		PET_MP_REGEN_MULTIPLIER = parser.getDouble("PetMpRegenMultiplier", 100) / 100;
		VITALITY_CONSUME_BY_MOB = parser.getInt("VitalityConsumeByMob", 2250);
		VITALITY_CONSUME_BY_BOSS = parser.getInt("VitalityConsumeByBoss", 1125);

		// Load Rates config file (if exists)
		parser.LoadConfig(RATES_CONFIG_FILE);
		RATE_XP = parser.getFloat("RateXp", 1);
		RATE_SP = parser.getFloat("RateSp", 1);
		RATE_PARTY_XP = parser.getFloat("RatePartyXp", 1);
		RATE_PARTY_SP = parser.getFloat("RatePartySp", 1);
		RATE_INSTANCE_XP = parser.getFloat("RateInstanceXp", -1);
		if (RATE_INSTANCE_XP < 0)
		{
			RATE_INSTANCE_XP = RATE_XP;
		}

		RATE_INSTANCE_SP = parser.getFloat("RateInstanceSp", -1);
		if (RATE_INSTANCE_SP < 0)
		{
			RATE_INSTANCE_SP = RATE_SP;
		}

		RATE_INSTANCE_PARTY_XP = parser.getFloat("RateInstancePartyXp", -1);
		if (RATE_INSTANCE_PARTY_XP < 0)
		{
			RATE_INSTANCE_PARTY_XP = RATE_PARTY_XP;
		}

		RATE_INSTANCE_PARTY_SP = parser.getFloat("RateInstancePartyXp", -1);
		if (RATE_INSTANCE_PARTY_SP < 0)
		{
			RATE_INSTANCE_PARTY_SP = RATE_PARTY_SP;
		}

		RATE_EXTRACTABLE = parser.getFloat("RateExtractable", 1);
		RATE_DROP_MANOR = parser.getInt("RateDropManor", 1);
		RATE_QUEST_DROP = parser.getFloat("RateQuestDrop", 1);
		RATE_QUEST_REWARD = parser.getFloat("RateQuestReward", 1);
		RATE_QUEST_REWARD_XP = parser.getFloat("RateQuestRewardXP", 1);
		RATE_QUEST_REWARD_SP = parser.getFloat("RateQuestRewardSP", 1);
		RATE_QUEST_REWARD_ADENA = parser.getFloat("RateQuestRewardAdena", 1);
		RATE_QUEST_REWARD_USE_MULTIPLIERS = parser.getBoolean("UseQuestRewardMultipliers");
		RATE_QUEST_REWARD_POTION = parser.getFloat("RateQuestRewardPotion", 1);
		RATE_QUEST_REWARD_SCROLL = parser.getFloat("RateQuestRewardScroll", 1);
		RATE_QUEST_REWARD_RECIPE = parser.getFloat("RateQuestRewardRecipe", 1);
		RATE_QUEST_REWARD_MATERIAL = parser.getFloat("RateQuestRewardMaterial", 1);
		RATE_RAIDBOSS_POINTS = parser.getFloat("RateRaidbossPointsReward", 1);
		RATE_VITALITY_EXP_MULTIPLIER = parser.getFloat("RateVitalityExpMultiplier", 2);
		RATE_LIMITED_SAYHA_GRACE_EXP_MULTIPLIER = parser.getFloat("RateLimitedSayhaGraceExpMultiplier", 2);
		VITALITY_MAX_ITEMS_ALLOWED = parser.getInt("VitalityMaxItemsAllowed", 999);
		if (VITALITY_MAX_ITEMS_ALLOWED == 0)
		{
			VITALITY_MAX_ITEMS_ALLOWED = int.MaxValue;
		}

		RATE_VITALITY_LOST = parser.getFloat("RateVitalityLost", 1);
		RATE_VITALITY_GAIN = parser.getFloat("RateVitalityGain", 1);
		RATE_KARMA_LOST = parser.getFloat("RateKarmaLost", -1);
		if (RATE_KARMA_LOST == -1)
		{
			RATE_KARMA_LOST = RATE_XP;
		}

		RATE_KARMA_EXP_LOST = parser.getFloat("RateKarmaExpLost", 1);
		RATE_SIEGE_GUARDS_PRICE = parser.getFloat("RateSiegeGuardsPrice", 1);
		PLAYER_DROP_LIMIT = parser.getInt("PlayerDropLimit", 3);
		PLAYER_RATE_DROP = parser.getInt("PlayerRateDrop", 5);
		PLAYER_RATE_DROP_ITEM = parser.getInt("PlayerRateDropItem", 70);
		PLAYER_RATE_DROP_EQUIP = parser.getInt("PlayerRateDropEquip", 25);
		PLAYER_RATE_DROP_EQUIP_WEAPON = parser.getInt("PlayerRateDropEquipWeapon", 5);
		PET_XP_RATE = parser.getFloat("PetXpRate", 1);
		PET_FOOD_RATE = parser.getInt("PetFoodRate", 1);
		SINEATER_XP_RATE = parser.getFloat("SinEaterXpRate", 1);
		KARMA_DROP_LIMIT = parser.getInt("KarmaDropLimit", 10);
		KARMA_RATE_DROP = parser.getInt("KarmaRateDrop", 70);
		KARMA_RATE_DROP_ITEM = parser.getInt("KarmaRateDropItem", 50);
		KARMA_RATE_DROP_EQUIP = parser.getInt("KarmaRateDropEquip", 40);
		KARMA_RATE_DROP_EQUIP_WEAPON = parser.getInt("KarmaRateDropEquipWeapon", 10);
		RATE_DEATH_DROP_AMOUNT_MULTIPLIER = parser.getFloat("DeathDropAmountMultiplier", 1);
		RATE_SPOIL_DROP_AMOUNT_MULTIPLIER = parser.getFloat("SpoilDropAmountMultiplier", 1);
		RATE_HERB_DROP_AMOUNT_MULTIPLIER = parser.getFloat("HerbDropAmountMultiplier", 1);
		RATE_RAID_DROP_AMOUNT_MULTIPLIER = parser.getFloat("RaidDropAmountMultiplier", 1);
		RATE_DEATH_DROP_CHANCE_MULTIPLIER = parser.getFloat("DeathDropChanceMultiplier", 1);
		RATE_SPOIL_DROP_CHANCE_MULTIPLIER = parser.getFloat("SpoilDropChanceMultiplier", 1);
		RATE_HERB_DROP_CHANCE_MULTIPLIER = parser.getFloat("HerbDropChanceMultiplier", 1);
		RATE_RAID_DROP_CHANCE_MULTIPLIER = parser.getFloat("RaidDropChanceMultiplier", 1);
		RATE_DROP_AMOUNT_BY_ID = parser.GetIdValueMap<float>("DropAmountMultiplierByItemId");
		RATE_DROP_CHANCE_BY_ID = parser.GetIdValueMap<float>("DropChanceMultiplierByItemId");
		DROP_MAX_OCCURRENCES_NORMAL = parser.getInt("DropMaxOccurrencesNormal", 2);
		DROP_MAX_OCCURRENCES_RAIDBOSS = parser.getInt("DropMaxOccurrencesRaidboss", 7);
		DROP_ADENA_MAX_LEVEL_LOWEST_DIFFERENCE = parser.getInt("DropAdenaMaxLevelLowestDifference", 14);
		DROP_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE = parser.getInt("DropItemMaxLevelLowestDifference", 14);
		EVENT_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE = parser.getInt("EventItemMaxLevelLowestDifference", 14);
		BLESSING_CHANCE = parser.getDouble("BlessingChance", 15.0);
		BOSS_DROP_ENABLED = parser.getBoolean("BossDropEnable");
		BOSS_DROP_MIN_LEVEL = parser.getInt("BossDropMinLevel", 40);
		BOSS_DROP_MAX_LEVEL = parser.getInt("BossDropMaxLevel", 999);
		BOSS_DROP_LIST = GetDropList(parser, "BossDropList");
		LCOIN_DROP_ENABLED = parser.getBoolean("LCoinDropEnable");
		LCOIN_DROP_CHANCE = parser.getDouble("LCoinDropChance", 15.0);
		LCOIN_MIN_MOB_LEVEL = parser.getInt("LCoinMinimumMonsterLevel", 40);
		LCOIN_MIN_QUANTITY = parser.getInt("LCoinMinDropQuantity", 1);
		LCOIN_MAX_QUANTITY = parser.getInt("LCoinMaxDropQuantity", 5);

		// Load PvP config file (if exists)
		parser.LoadConfig(PVP_CONFIG_FILE);
		KARMA_DROP_GM = parser.getBoolean("CanGMDropEquipment");
		KARMA_PK_LIMIT = parser.getInt("MinimumPKRequiredToDrop", 4);
		KARMA_NONDROPPABLE_PET_ITEMS = parser.GetIntList("ListOfPetItems", ',', 2375, 3500, 3501, 3502, 4422, 4423,
			4424, 4425, 6648, 6649, 6650, 9882).ToImmutableSortedSet();

		KARMA_NONDROPPABLE_ITEMS = parser.GetIntList("ListOfNonDroppableItems", ',', 57, 1147, 425, 1146, 461, 10, 2368,
			7, 6, 2370, 2369, 6842, 6611, 6612, 6613, 6614, 6615, 6616, 6617, 6618, 6619, 6620, 6621, 7694, 8181, 5575,
			7694, 9388, 9389, 9390).ToImmutableSortedSet();
		ANTIFEED_ENABLE = parser.getBoolean("AntiFeedEnable");
		ANTIFEED_DUALBOX = parser.getBoolean("AntiFeedDualbox", true);
		ANTIFEED_DISCONNECTED_AS_DUALBOX = parser.getBoolean("AntiFeedDisconnectedAsDualbox", true);
		ANTIFEED_INTERVAL = parser.getInt("AntiFeedInterval", 120) * 1000;
		VAMPIRIC_ATTACK_AFFECTS_PVP = parser.getBoolean("VampiricAttackAffectsPvP");
		MP_VAMPIRIC_ATTACK_AFFECTS_PVP = parser.getBoolean("MpVampiricAttackAffectsPvP");
		PVP_NORMAL_TIME = TimeSpan.FromMilliseconds(parser.getInt("PvPVsNormalTime", 120000));
		PVP_PVP_TIME = TimeSpan.FromMilliseconds(parser.getInt("PvPVsPvPTime", 60000));
		MAX_REPUTATION = parser.getInt("MaxReputation", 500);
		REPUTATION_INCREASE = parser.getInt("ReputationIncrease", 100);

		// Load Olympiad config file (if exists)
		parser.LoadConfig(OLYMPIAD_CONFIG_FILE);
		OLYMPIAD_ENABLED = parser.getBoolean("OlympiadEnabled", true);
		ALT_OLY_START_TIME = parser.getInt("AltOlyStartTime", 20);
		ALT_OLY_MIN = parser.getInt("AltOlyMin");
		ALT_OLY_CPERIOD = parser.getLong("AltOlyCPeriod", 14400000);
		ALT_OLY_BATTLE = parser.getLong("AltOlyBattle", 300000);
		ALT_OLY_WPERIOD = parser.getLong("AltOlyWPeriod", 604800000);
		ALT_OLY_VPERIOD = parser.getLong("AltOlyVPeriod", 86400000);
		ALT_OLY_START_POINTS = parser.getInt("AltOlyStartPoints", 10);
		ALT_OLY_WEEKLY_POINTS = parser.getInt("AltOlyWeeklyPoints", 10);
		ALT_OLY_CLASSED = parser.getInt("AltOlyClassedParticipants", 10);
		ALT_OLY_NONCLASSED = parser.getInt("AltOlyNonClassedParticipants", 20);
		ALT_OLY_WINNER_REWARD = parser.GetIdValueMap<int>("AltOlyWinReward").Select(x => new ItemHolder(x.Key, x.Value)).ToImmutableArray();
		ALT_OLY_LOSER_REWARD = parser.GetIdValueMap<int>("AltOlyLoserReward").Select(x => new ItemHolder(x.Key, x.Value)).ToImmutableArray();
		ALT_OLY_COMP_RITEM = parser.getInt("AltOlyCompRewItem", 45584);
		ALT_OLY_MIN_MATCHES = parser.getInt("AltOlyMinMatchesForPoints", 10);
		ALT_OLY_MARK_PER_POINT = parser.getInt("AltOlyMarkPerPoint", 20);
		ALT_OLY_HERO_POINTS = parser.getInt("AltOlyHeroPoints", 30);
		ALT_OLY_RANK1_POINTS = parser.getInt("AltOlyRank1Points", 60);
		ALT_OLY_RANK2_POINTS = parser.getInt("AltOlyRank2Points", 50);
		ALT_OLY_RANK3_POINTS = parser.getInt("AltOlyRank3Points", 45);
		ALT_OLY_RANK4_POINTS = parser.getInt("AltOlyRank4Points", 40);
		ALT_OLY_RANK5_POINTS = parser.getInt("AltOlyRank5Points", 30);
		ALT_OLY_MAX_POINTS = parser.getInt("AltOlyMaxPoints", 10);
		ALT_OLY_DIVIDER_CLASSED = parser.getInt("AltOlyDividerClassed", 5);
		ALT_OLY_DIVIDER_NON_CLASSED = parser.getInt("AltOlyDividerNonClassed", 5);
		ALT_OLY_MAX_WEEKLY_MATCHES = parser.getInt("AltOlyMaxWeeklyMatches", 30);
		ALT_OLY_LOG_FIGHTS = parser.getBoolean("AltOlyLogFights");
		ALT_OLY_SHOW_MONTHLY_WINNERS = parser.getBoolean("AltOlyShowMonthlyWinners", true);
		ALT_OLY_ANNOUNCE_GAMES = parser.getBoolean("AltOlyAnnounceGames", true);
		LIST_OLY_RESTRICTED_ITEMS = parser.GetIntList("AltOlyRestrictedItems").ToImmutableSortedSet();
		ALT_OLY_WEAPON_ENCHANT_LIMIT = parser.getInt("AltOlyWeaponEnchantLimit", -1);
		ALT_OLY_ARMOR_ENCHANT_LIMIT = parser.getInt("AltOlyArmorEnchantLimit", -1);
		ALT_OLY_WAIT_TIME = parser.getInt("AltOlyWaitTime", 60);
		ALT_OLY_PERIOD = parser.getString("AltOlyPeriod", "MONTH");
		ALT_OLY_PERIOD_MULTIPLIER = parser.getInt("AltOlyPeriodMultiplier", 1);
		ALT_OLY_COMPETITION_DAYS = parser.GetIntList("AltOlyCompetitionDays").Select(x => (DayOfWeek)x).ToImmutableSortedSet();

		// TODO: instead of HEXID_FILE, game server can use token to access Login server

		// Grand bosses
		parser.LoadConfig(GRANDBOSS_CONFIG_FILE);
		ANTHARAS_WAIT_TIME = parser.getInt("AntharasWaitTime", 30);
		ANTHARAS_SPAWN_INTERVAL = parser.getInt("IntervalOfAntharasSpawn", 264);
		ANTHARAS_SPAWN_RANDOM = parser.getInt("RandomOfAntharasSpawn", 72);
		BAIUM_SPAWN_INTERVAL = parser.getInt("IntervalOfBaiumSpawn", 168);
		CORE_SPAWN_INTERVAL = parser.getInt("IntervalOfCoreSpawn", 60);
		CORE_SPAWN_RANDOM = parser.getInt("RandomOfCoreSpawn", 24);
		ORFEN_SPAWN_INTERVAL = parser.getInt("IntervalOfOrfenSpawn", 48);
		ORFEN_SPAWN_RANDOM = parser.getInt("RandomOfOrfenSpawn", 20);
		QUEEN_ANT_SPAWN_INTERVAL = parser.getInt("IntervalOfQueenAntSpawn", 36);
		QUEEN_ANT_SPAWN_RANDOM = parser.getInt("RandomOfQueenAntSpawn", 17);
		ZAKEN_SPAWN_INTERVAL = parser.getInt("IntervalOfZakenSpawn", 168);
		ZAKEN_SPAWN_RANDOM = parser.getInt("RandomOfZakenSpawn", 48);
		BALOK_TIME = parser.GetTime("BalokTime", TimeOnly.FromTimeSpan(new TimeSpan(20, 30, 0)));
		BALOK_POINTS_PER_MONSTER = parser.getInt("BalokPointsPerMonster", 10);

		// Load HuntPass (if exists)
		parser.LoadConfig(HUNT_PASS_CONFIG_FILE);
		ENABLE_HUNT_PASS = parser.getBoolean("EnabledHuntPass", true);
		HUNT_PASS_PREMIUM_ITEM_ID = parser.getInt("PremiumItemId", 91663);
		HUNT_PASS_PREMIUM_ITEM_COUNT = parser.getInt("PremiumItemCount", 3600);
		HUNT_PASS_POINTS_FOR_STEP = parser.getInt("PointsForStep", 2400);
		HUNT_PASS_PERIOD = parser.getInt("DayOfMonth", 1);

		// Load ArchivementBox (if exists)
		parser.LoadConfig(ACHIEVEMENT_BOX_CONFIG_FILE);
		ENABLE_ACHIEVEMENT_BOX = parser.getBoolean("EnabledAchievementBox", true);
		ACHIEVEMENT_BOX_POINTS_FOR_REWARD = parser.getInt("PointsForReward", 1000);
		ENABLE_ACHIEVEMENT_PVP = parser.getBoolean("EnabledAchievementPvP", true);
		ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD = parser.getInt("PointsForPvpReward", 5);

		// Orc Fortress
		parser.LoadConfig(ORC_FORTRESS_CONFIG_FILE);
		ORC_FORTRESS_TIME = parser.GetTime("OrcFortressTime", TimeOnly.FromTimeSpan(new TimeSpan(20, 0, 0)));
		ORC_FORTRESS_ENABLE = parser.getBoolean("OrcFortressEnable", true);

		// Gracia Seeds
		parser.LoadConfig(GRACIASEEDS_CONFIG_FILE);
		SOD_TIAT_KILL_COUNT = parser.getInt("TiatKillCountForNextState", 10);
		SOD_STAGE_2_LENGTH = parser.getLong("Stage2Length", 720) * 60000;

		FILTER_LIST = File.Exists(CHAT_FILTER_FILE)
			? File.ReadLines(CHAT_FILTER_FILE, Encoding.UTF8).Select(s => s.Trim())
				.Where(s => !string.IsNullOrEmpty(s) && s[0] != '#').ToImmutableArray()
			: ImmutableArray<string>.Empty;

		// Load GeoEngine config file (if exists)
		parser.LoadConfig(GEOENGINE_CONFIG_FILE);
		GEODATA_PATH = Path.Combine(dataPackPath, parser.getString("GeoDataPath", "geodata"));
		PATHNODE_PATH = Path.Combine(dataPackPath, parser.getString("PathnodePath", "pathnode"));
		GEOEDIT_PATH = Path.Combine(dataPackPath, parser.getString("GeoEditPath", "geodata_save"));
		PATHFINDING = parser.getInt("PathFinding");
		PATHFIND_BUFFERS = parser.getString("PathFindBuffers", "100x6;128x6;192x6;256x4;320x4;384x4;500x2");
		LOW_WEIGHT = parser.getFloat("LowWeight", 0.5f);
		MEDIUM_WEIGHT = parser.getFloat("MediumWeight", 2);
		HIGH_WEIGHT = parser.getFloat("HighWeight", 3);
		ADVANCED_DIAGONAL_STRATEGY = parser.getBoolean("AdvancedDiagonalStrategy", true);
		DIAGONAL_WEIGHT = parser.getFloat("DiagonalWeight", 0.707f);
		MAX_POSTFILTER_PASSES = parser.getInt("MaxPostfilterPasses", 3);
		DEBUG_PATH = parser.getBoolean("DebugPath");

		// Load AllowedPlayerRaces config file (if exists)
		parser.LoadConfig(CUSTOM_ALLOWED_PLAYER_RACES_CONFIG_FILE);
		ALLOW_HUMAN = parser.getBoolean("AllowHuman", true);
		ALLOW_ELF = parser.getBoolean("AllowElf", true);
		ALLOW_DARKELF = parser.getBoolean("AllowDarkElf", true);
		ALLOW_ORC = parser.getBoolean("AllowOrc", true);
		ALLOW_DWARF = parser.getBoolean("AllowDwarf", true);
		ALLOW_KAMAEL = parser.getBoolean("AllowKamael", true);
		ALLOW_DEATH_KNIGHT = parser.getBoolean("AllowDeathKnight", true);
		ALLOW_SYLPH = parser.getBoolean("AllowSylph", true);
		ALLOW_VANGUARD = parser.getBoolean("AllowVanguard", true);

		// Load AutoPotions config file (if exists)
		parser.LoadConfig(CUSTOM_AUTO_POTIONS_CONFIG_FILE);
		AUTO_POTIONS_ENABLED = parser.getBoolean("AutoPotionsEnabled");
		AUTO_POTIONS_IN_OLYMPIAD = parser.getBoolean("AutoPotionsInOlympiad");
		AUTO_POTION_MIN_LEVEL = parser.getInt("AutoPotionMinimumLevel", 1);
		AUTO_CP_ENABLED = parser.getBoolean("AutoCpEnabled", true);
		AUTO_HP_ENABLED = parser.getBoolean("AutoHpEnabled", true);
		AUTO_MP_ENABLED = parser.getBoolean("AutoMpEnabled", true);
		AUTO_CP_PERCENTAGE = parser.getInt("AutoCpPercentage", 70);
		AUTO_HP_PERCENTAGE = parser.getInt("AutoHpPercentage", 70);
		AUTO_MP_PERCENTAGE = parser.getInt("AutoMpPercentage", 70);
		AUTO_CP_ITEM_IDS = parser.GetIntList("AutoCpItemIds").ToImmutableSortedSet();
		AUTO_HP_ITEM_IDS = parser.GetIntList("AutoHpItemIds").ToImmutableSortedSet();
		AUTO_MP_ITEM_IDS = parser.GetIntList("AutoMpItemIds").ToImmutableSortedSet();

		// Load Banking config file (if exists)
		parser.LoadConfig(CUSTOM_BANKING_CONFIG_FILE);
		BANKING_SYSTEM_ENABLED = parser.getBoolean("BankingEnabled");
		BANKING_SYSTEM_GOLDBARS = parser.getInt("BankingGoldbarCount", 1);
		BANKING_SYSTEM_ADENA = parser.getInt("BankingAdenaCount", 500000000);

		// Load Boss Announcements config file (if exists)
		parser.LoadConfig(CUSTOM_BOSS_ANNOUNCEMENTS_CONFIG_FILE);
		RAIDBOSS_SPAWN_ANNOUNCEMENTS = parser.getBoolean("RaidBossSpawnAnnouncements");
		RAIDBOSS_DEFEAT_ANNOUNCEMENTS = parser.getBoolean("RaidBossDefeatAnnouncements");
		RAIDBOSS_INSTANCE_ANNOUNCEMENTS = parser.getBoolean("RaidBossInstanceAnnouncements");
		GRANDBOSS_SPAWN_ANNOUNCEMENTS = parser.getBoolean("GrandBossSpawnAnnouncements");
		GRANDBOSS_DEFEAT_ANNOUNCEMENTS = parser.getBoolean("GrandBossDefeatAnnouncements");
		GRANDBOSS_INSTANCE_ANNOUNCEMENTS = parser.getBoolean("GrandBossInstanceAnnouncements");

		// Load BoostNpcStats config file (if exists)
		parser.LoadConfig(CUSTOM_NPC_STAT_MULTIPLIERS_CONFIG_FILE);
		ENABLE_NPC_STAT_MULTIPLIERS = parser.getBoolean("EnableNpcStatMultipliers");
		MONSTER_HP_MULTIPLIER = parser.getDouble("MonsterHP", 1.0);
		MONSTER_MP_MULTIPLIER = parser.getDouble("MonsterMP", 1.0);
		MONSTER_PATK_MULTIPLIER = parser.getDouble("MonsterPAtk", 1.0);
		MONSTER_MATK_MULTIPLIER = parser.getDouble("MonsterMAtk", 1.0);
		MONSTER_PDEF_MULTIPLIER = parser.getDouble("MonsterPDef", 1.0);
		MONSTER_MDEF_MULTIPLIER = parser.getDouble("MonsterMDef", 1.0);
		MONSTER_AGRRO_RANGE_MULTIPLIER = parser.getDouble("MonsterAggroRange", 1.0);
		MONSTER_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("MonsterClanHelpRange", 1.0);
		RAIDBOSS_HP_MULTIPLIER = parser.getDouble("RaidbossHP", 1.0);
		RAIDBOSS_MP_MULTIPLIER = parser.getDouble("RaidbossMP", 1.0);
		RAIDBOSS_PATK_MULTIPLIER = parser.getDouble("RaidbossPAtk", 1.0);
		RAIDBOSS_MATK_MULTIPLIER = parser.getDouble("RaidbossMAtk", 1.0);
		RAIDBOSS_PDEF_MULTIPLIER = parser.getDouble("RaidbossPDef", 1.0);
		RAIDBOSS_MDEF_MULTIPLIER = parser.getDouble("RaidbossMDef", 1.0);
		RAIDBOSS_AGRRO_RANGE_MULTIPLIER = parser.getDouble("RaidbossAggroRange", 1.0);
		RAIDBOSS_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("RaidbossClanHelpRange", 1.0);
		GUARD_HP_MULTIPLIER = parser.getDouble("GuardHP", 1.0);
		GUARD_MP_MULTIPLIER = parser.getDouble("GuardMP", 1.0);
		GUARD_PATK_MULTIPLIER = parser.getDouble("GuardPAtk", 1.0);
		GUARD_MATK_MULTIPLIER = parser.getDouble("GuardMAtk", 1.0);
		GUARD_PDEF_MULTIPLIER = parser.getDouble("GuardPDef", 1.0);
		GUARD_MDEF_MULTIPLIER = parser.getDouble("GuardMDef", 1.0);
		GUARD_AGRRO_RANGE_MULTIPLIER = parser.getDouble("GuardAggroRange", 1.0);
		GUARD_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("GuardClanHelpRange", 1.0);
		DEFENDER_HP_MULTIPLIER = parser.getDouble("DefenderHP", 1.0);
		DEFENDER_MP_MULTIPLIER = parser.getDouble("DefenderMP", 1.0);
		DEFENDER_PATK_MULTIPLIER = parser.getDouble("DefenderPAtk", 1.0);
		DEFENDER_MATK_MULTIPLIER = parser.getDouble("DefenderMAtk", 1.0);
		DEFENDER_PDEF_MULTIPLIER = parser.getDouble("DefenderPDef", 1.0);
		DEFENDER_MDEF_MULTIPLIER = parser.getDouble("DefenderMDef", 1.0);
		DEFENDER_AGRRO_RANGE_MULTIPLIER = parser.getDouble("DefenderAggroRange", 1.0);
		DEFENDER_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("DefenderClanHelpRange", 1.0);

		// Load ChampionMonster config file (if exists)
		parser.LoadConfig(CUSTOM_CHAMPION_MONSTERS_CONFIG_FILE);
		CHAMPION_ENABLE = parser.getBoolean("ChampionEnable");
		CHAMPION_PASSIVE = parser.getBoolean("ChampionPassive");
		CHAMPION_FREQUENCY = parser.getInt("ChampionFrequency");
		CHAMP_TITLE = parser.getString("ChampionTitle", "Champion");
		SHOW_CHAMPION_AURA = parser.getBoolean("ChampionAura", true);
		CHAMP_MIN_LEVEL = parser.getInt("ChampionMinLevel", 20);
		CHAMP_MAX_LEVEL = parser.getInt("ChampionMaxLevel", 60);
		CHAMPION_HP = parser.getInt("ChampionHp", 7);
		CHAMPION_HP_REGEN = parser.getFloat("ChampionHpRegen", 1);
		CHAMPION_REWARDS_EXP_SP = parser.getFloat("ChampionRewardsExpSp", 8);
		CHAMPION_REWARDS_CHANCE = parser.getFloat("ChampionRewardsChance", 8);
		CHAMPION_REWARDS_AMOUNT = parser.getFloat("ChampionRewardsAmount", 1);
		CHAMPION_ADENAS_REWARDS_CHANCE = parser.getFloat("ChampionAdenasRewardsChance", 1);
		CHAMPION_ADENAS_REWARDS_AMOUNT = parser.getFloat("ChampionAdenasRewardsAmount", 1);
		CHAMPION_ATK = parser.getFloat("ChampionAtk", 1);
		CHAMPION_SPD_ATK = parser.getFloat("ChampionSpdAtk", 1);
		CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE = parser.getInt("ChampionRewardLowerLvlItemChance");
		CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE = parser.getInt("ChampionRewardHigherLvlItemChance");
		CHAMPION_REWARD_ITEMS = parser.GetIdValueMap<int>("ChampionRewardItems");
		CHAMPION_ENABLE_VITALITY = parser.getBoolean("ChampionEnableVitality");
		CHAMPION_ENABLE_IN_INSTANCES = parser.getBoolean("ChampionEnableInInstances");

		// Load ChatModeration config file (if exists)
		parser.LoadConfig(CUSTOM_CHAT_MODERATION_CONFIG_FILE);
		CHAT_ADMIN = parser.getBoolean("ChatAdmin", true);

		// Load ClassBalance config file (if exists)
		parser.LoadConfig(CUSTOM_CLASS_BALANCE_CONFIG_FILE);
		PVE_MAGICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PveMagicalSkillDamageMultipliers");
		PVP_MAGICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpMagicalSkillDamageMultipliers");
		PVE_MAGICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PveMagicalSkillDefenceMultipliers");
		PVP_MAGICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpMagicalSkillDefenceMultipliers");
		PVE_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
			GetMultipliers(parser, "PveMagicalSkillCriticalChanceMultipliers");
		PVP_MAGICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
			GetMultipliers(parser, "PvpMagicalSkillCriticalChanceMultipliers");
		PVE_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
			GetMultipliers(parser, "PveMagicalSkillCriticalDamageMultipliers");
		PVP_MAGICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
			GetMultipliers(parser, "PvpMagicalSkillCriticalDamageMultipliers");
		PVE_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalSkillDamageMultipliers");
		PVP_PHYSICAL_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalSkillDamageMultipliers");
		PVE_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalSkillDefenceMultipliers");
		PVP_PHYSICAL_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalSkillDefenceMultipliers");
		PVE_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
			GetMultipliers(parser, "PvePhysicalSkillCriticalChanceMultipliers");
		PVP_PHYSICAL_SKILL_CRITICAL_CHANCE_MULTIPLIERS =
			GetMultipliers(parser, "PvpPhysicalSkillCriticalChanceMultipliers");
		PVE_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
			GetMultipliers(parser, "PvePhysicalSkillCriticalDamageMultipliers");
		PVP_PHYSICAL_SKILL_CRITICAL_DAMAGE_MULTIPLIERS =
			GetMultipliers(parser, "PvpPhysicalSkillCriticalDamageMultipliers");
		PVE_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalAttackDamageMultipliers");
		PVP_PHYSICAL_ATTACK_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalAttackDamageMultipliers");
		PVE_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvePhysicalAttackDefenceMultipliers");
		PVP_PHYSICAL_ATTACK_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpPhysicalAttackDefenceMultipliers");
		PVE_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
			GetMultipliers(parser, "PvePhysicalAttackCriticalChanceMultipliers");
		PVP_PHYSICAL_ATTACK_CRITICAL_CHANCE_MULTIPLIERS =
			GetMultipliers(parser, "PvpPhysicalAttackCriticalChanceMultipliers");
		PVE_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
			GetMultipliers(parser, "PvePhysicalAttackCriticalDamageMultipliers");
		PVP_PHYSICAL_ATTACK_CRITICAL_DAMAGE_MULTIPLIERS =
			GetMultipliers(parser, "PvpPhysicalAttackCriticalDamageMultipliers");
		PVE_BLOW_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PveBlowSkillDamageMultipliers");
		PVP_BLOW_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpBlowSkillDamageMultipliers");
		PVE_BLOW_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PveBlowSkillDefenceMultipliers");
		PVP_BLOW_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpBlowSkillDefenceMultipliers");
		PVE_ENERGY_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PveEnergySkillDamageMultipliers");
		PVP_ENERGY_SKILL_DAMAGE_MULTIPLIERS = GetMultipliers(parser, "PvpEnergySkillDamageMultipliers");
		PVE_ENERGY_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PveEnergySkillDefenceMultipliers");
		PVP_ENERGY_SKILL_DEFENCE_MULTIPLIERS = GetMultipliers(parser, "PvpEnergySkillDefenceMultipliers");
		PLAYER_HEALING_SKILL_MULTIPLIERS = GetMultipliers(parser, "PlayerHealingSkillMultipliers");
		SKILL_MASTERY_CHANCE_MULTIPLIERS = GetMultipliers(parser, "SkillMasteryChanceMultipliers");
		EXP_AMOUNT_MULTIPLIERS = GetMultipliers(parser, "ExpAmountMultipliers");
		SP_AMOUNT_MULTIPLIERS = GetMultipliers(parser, "SpAmountMultipliers");

		// Load CommunityBoard config file (if exists)
		parser.LoadConfig(CUSTOM_COMMUNITY_BOARD_CONFIG_FILE);
		CUSTOM_CB_ENABLED = parser.getBoolean("CustomCommunityBoard");
		COMMUNITYBOARD_CURRENCY = parser.getInt("CommunityCurrencyId", 57);
		COMMUNITYBOARD_ENABLE_MULTISELLS = parser.getBoolean("CommunityEnableMultisells", true);
		COMMUNITYBOARD_ENABLE_TELEPORTS = parser.getBoolean("CommunityEnableTeleports", true);
		COMMUNITYBOARD_ENABLE_BUFFS = parser.getBoolean("CommunityEnableBuffs", true);
		COMMUNITYBOARD_ENABLE_HEAL = parser.getBoolean("CommunityEnableHeal", true);
		COMMUNITYBOARD_ENABLE_DELEVEL = parser.getBoolean("CommunityEnableDelevel");
		COMMUNITYBOARD_TELEPORT_PRICE = parser.getInt("CommunityTeleportPrice");
		COMMUNITYBOARD_BUFF_PRICE = parser.getInt("CommunityBuffPrice");
		COMMUNITYBOARD_HEAL_PRICE = parser.getInt("CommunityHealPrice");
		COMMUNITYBOARD_DELEVEL_PRICE = parser.getInt("CommunityDelevelPrice");
		COMMUNITYBOARD_KARMA_DISABLED = parser.getBoolean("CommunityKarmaDisabled", true);
		COMMUNITYBOARD_CAST_ANIMATIONS = parser.getBoolean("CommunityCastAnimations");
		COMMUNITY_PREMIUM_SYSTEM_ENABLED = parser.getBoolean("CommunityPremiumSystem");
		COMMUNITY_PREMIUM_COIN_ID = parser.getInt("CommunityPremiumBuyCoinId", 57);
		COMMUNITY_PREMIUM_PRICE_PER_DAY = parser.getInt("CommunityPremiumPricePerDay", 1000000);
		COMMUNITY_AVAILABLE_BUFFS = parser.GetIntList("CommunityAvailableBuffs").ToImmutableSortedSet();
		COMMUNITY_AVAILABLE_TELEPORTS = GetLocations(parser, "CommunityTeleportList");

		// Load CustomDepositableItems config file (if exists)
		parser.LoadConfig(CUSTOM_CUSTOM_DEPOSITABLE_ITEMS_CONFIG_FILE);
		CUSTOM_DEPOSITABLE_ENABLED = parser.getBoolean("CustomDepositableEnabled");
		CUSTOM_DEPOSITABLE_QUEST_ITEMS = parser.getBoolean("DepositableQuestItems");

		// Load CustomMailManager config file (if exists)
		parser.LoadConfig(CUSTOM_CUSTOM_MAIL_MANAGER_CONFIG_FILE);
		CUSTOM_MAIL_MANAGER_ENABLED = parser.getBoolean("CustomMailManagerEnabled");
		CUSTOM_MAIL_MANAGER_DELAY = parser.getInt("DatabaseQueryDelay", 30) * 1000;

		// Load DelevelManager config file (if exists)
		parser.LoadConfig(CUSTOM_DELEVEL_MANAGER_CONFIG_FILE);
		DELEVEL_MANAGER_ENABLED = parser.getBoolean("Enabled");
		DELEVEL_MANAGER_NPCID = parser.getInt("NpcId", 1002000);
		DELEVEL_MANAGER_ITEMID = parser.getInt("RequiredItemId", 4356);
		DELEVEL_MANAGER_ITEMCOUNT = parser.getInt("RequiredItemCount", 2);
		DELEVEL_MANAGER_MINIMUM_DELEVEL = parser.getInt("MimimumDelevel", 20);

		// Load DualboxCheck config file (if exists)
		parser.LoadConfig(CUSTOM_DUALBOX_CHECK_CONFIG_FILE);
		DUALBOX_CHECK_MAX_PLAYERS_PER_IP = parser.getInt("DualboxCheckMaxPlayersPerIP");
		DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP = parser.getInt("DualboxCheckMaxOlympiadParticipantsPerIP");
		DUALBOX_CHECK_MAX_L2EVENT_PARTICIPANTS_PER_IP = parser.getInt("DualboxCheckMaxL2EventParticipantsPerIP");
		DUALBOX_COUNT_OFFLINE_TRADERS = parser.getBoolean("DualboxCountOfflineTraders");
		// DUALBOX_CHECK_WHITELIST = parser.getString("DualboxCheckWhitelist", "127.0.0.1,0"); // TODO: implement

		// Load FactionSystem config file (if exists)
		parser.LoadConfig(CUSTOM_FACTION_SYSTEM_CONFIG_FILE);
		FACTION_SYSTEM_ENABLED = parser.getBoolean("EnableFactionSystem");
		FACTION_STARTING_LOCATION = GetLocation(parser, "StartingLocation", 85332, 16199, -1252);
		FACTION_MANAGER_LOCATION = GetLocation(parser, "ManagerSpawnLocation", 85712, 15974, -1260, 26808);
		FACTION_GOOD_BASE_LOCATION = GetLocation(parser, "GoodBaseLocation", 45306, 48878, -3058);
		FACTION_EVIL_BASE_LOCATION = GetLocation(parser, "EvilBaseLocation", -44037, -113283, -237);
		FACTION_GOOD_TEAM_NAME = parser.getString("GoodTeamName", "Good");
		FACTION_EVIL_TEAM_NAME = parser.getString("EvilTeamName", "Evil");
		FACTION_GOOD_NAME_COLOR = parser.GetColor("GoodNameColor", new Color(0x00FF00));
		FACTION_EVIL_NAME_COLOR = parser.GetColor("EvilNameColor", new Color(0x0000FF));
		FACTION_GUARDS_ENABLED = parser.getBoolean("EnableFactionGuards", true);
		FACTION_RESPAWN_AT_BASE = parser.getBoolean("RespawnAtFactionBase", true);
		FACTION_AUTO_NOBLESS = parser.getBoolean("FactionAutoNobless");
		FACTION_SPECIFIC_CHAT = parser.getBoolean("EnableFactionChat", true);
		FACTION_BALANCE_ONLINE_PLAYERS = parser.getBoolean("BalanceOnlinePlayers", true);
		FACTION_BALANCE_PLAYER_EXCEED_LIMIT = parser.getInt("BalancePlayerExceedLimit", 20);

		// Load FakePlayers config file (if exists)
		parser.LoadConfig(CUSTOM_FAKE_PLAYERS_CONFIG_FILE);
		FAKE_PLAYERS_ENABLED = parser.getBoolean("EnableFakePlayers");
		FAKE_PLAYER_CHAT = parser.getBoolean("FakePlayerChat");
		FAKE_PLAYER_USE_SHOTS = parser.getBoolean("FakePlayerUseShots");
		FAKE_PLAYER_KILL_PVP = parser.getBoolean("FakePlayerKillsRewardPvP");
		FAKE_PLAYER_KILL_KARMA = parser.getBoolean("FakePlayerUnflaggedKillsKarma");
		FAKE_PLAYER_AUTO_ATTACKABLE = parser.getBoolean("FakePlayerAutoAttackable");
		FAKE_PLAYER_AGGRO_MONSTERS = parser.getBoolean("FakePlayerAggroMonsters");
		FAKE_PLAYER_AGGRO_PLAYERS = parser.getBoolean("FakePlayerAggroPlayers");
		FAKE_PLAYER_AGGRO_FPC = parser.getBoolean("FakePlayerAggroFPC");
		FAKE_PLAYER_CAN_DROP_ITEMS = parser.getBoolean("FakePlayerCanDropItems");
		FAKE_PLAYER_CAN_PICKUP = parser.getBoolean("FakePlayerCanPickup");

		// Load FindPvP config file (if exists)
		parser.LoadConfig(CUSTOM_FIND_PVP_CONFIG_FILE);
		ENABLE_FIND_PVP = parser.getBoolean("EnableFindPvP");

		// Load MerchantZeroSellPrice config file (if exists)
		parser.LoadConfig(CUSTOM_MERCHANT_ZERO_SELL_PRICE_CONFIG_FILE);
		MERCHANT_ZERO_SELL_PRICE = parser.getBoolean("MerchantZeroSellPrice");

		// Load MultilingualSupport config file (if exists)
		parser.LoadConfig(CUSTOM_MULTILANGUAL_SUPPORT_CONFIG_FILE);
		MULTILANG_DEFAULT = parser.getString("MultiLangDefault", "en").ToLower();
		MULTILANG_ENABLE = parser.getBoolean("MultiLangEnable");
		if (MULTILANG_ENABLE)
		{
			CHECK_HTML_ENCODING = false;
		}

		MULTILANG_ALLOWED = parser.GetStringList("MultiLangAllowed", ';', MULTILANG_DEFAULT);
		if (!MULTILANG_ALLOWED.Contains(MULTILANG_DEFAULT))
		{
			LOGGER.Error($"Default language missing in entry 'MultiLangAllowed' in configuration file '{parser.FilePath}'");
		}

		MULTILANG_VOICED_ALLOW = parser.getBoolean("MultiLangVoiceCommand", true);

		// Load NoblessMaster config file (if exists)
		parser.LoadConfig(CUSTOM_NOBLESS_MASTER_CONFIG_FILE);
		NOBLESS_MASTER_ENABLED = parser.getBoolean("Enabled");
		NOBLESS_MASTER_NPCID = parser.getInt("NpcId", 1003000);
		NOBLESS_MASTER_LEVEL_REQUIREMENT = parser.getInt("LevelRequirement", 80);
		NOBLESS_MASTER_REWARD_TIARA = parser.getBoolean("RewardTiara");

		// Load OfflinePlay config file (if exists)
		parser.LoadConfig(CUSTOM_OFFLINE_PLAY_CONFIG_FILE);
		ENABLE_OFFLINE_PLAY_COMMAND = parser.getBoolean("EnableOfflinePlayCommand");
		OFFLINE_PLAY_PREMIUM = parser.getBoolean("OfflinePlayPremium");
		OFFLINE_PLAY_LOGOUT_ON_DEATH = parser.getBoolean("OfflinePlayLogoutOnDeath", true);
		OFFLINE_PLAY_LOGIN_MESSAGE = parser.getString("OfflinePlayLoginMessage");
		OFFLINE_PLAY_SET_NAME_COLOR = parser.getBoolean("OfflinePlaySetNameColor");
		OFFLINE_PLAY_NAME_COLOR = parser.GetColor("OfflinePlayNameColor", new Color(0x808080));
		OFFLINE_PLAY_ABNORMAL_EFFECTS = parser.GetEnumList<AbnormalVisualEffect>("OfflinePlayAbnormalEffect");

		// Load OfflineTrade config file (if exists)
		parser.LoadConfig(CUSTOM_OFFLINE_TRADE_CONFIG_FILE);
		OFFLINE_TRADE_ENABLE = parser.getBoolean("OfflineTradeEnable");
		OFFLINE_CRAFT_ENABLE = parser.getBoolean("OfflineCraftEnable");
		OFFLINE_MODE_IN_PEACE_ZONE = parser.getBoolean("OfflineModeInPeaceZone");
		OFFLINE_MODE_NO_DAMAGE = parser.getBoolean("OfflineModeNoDamage");
		OFFLINE_SET_NAME_COLOR = parser.getBoolean("OfflineSetNameColor");
		OFFLINE_NAME_COLOR = parser.GetColor("OfflineNameColor", new Color(0x808080));
		OFFLINE_FAME = parser.getBoolean("OfflineFame", true);
		RESTORE_OFFLINERS = parser.getBoolean("RestoreOffliners");
		OFFLINE_MAX_DAYS = parser.getInt("OfflineMaxDays", 10);
		OFFLINE_DISCONNECT_FINISHED = parser.getBoolean("OfflineDisconnectFinished", true);
		OFFLINE_DISCONNECT_SAME_ACCOUNT = parser.getBoolean("OfflineDisconnectSameAccount");
		STORE_OFFLINE_TRADE_IN_REALTIME = parser.getBoolean("StoreOfflineTradeInRealtime", true);
		ENABLE_OFFLINE_COMMAND = parser.getBoolean("EnableOfflineCommand", true);
		OFFLINE_ABNORMAL_EFFECTS = parser.GetEnumList<AbnormalVisualEffect>("OfflineAbnormalEffect");

		// Load OnlineInfo config file (if exists)
		parser.LoadConfig(CUSTOM_ONLINE_INFO_CONFIG_FILE);
		ENABLE_ONLINE_COMMAND = parser.getBoolean("EnableOnlineCommand");

		// Load PasswordChange config file (if exists)
		parser.LoadConfig(CUSTOM_PASSWORD_CHANGE_CONFIG_FILE);
		ALLOW_CHANGE_PASSWORD = parser.getBoolean("AllowChangePassword");

		parser.LoadConfig(CUSTOM_VIP_CONFIG_FILE);
		VIP_SYSTEM_ENABLED = parser.getBoolean("VipEnabled");
		if (VIP_SYSTEM_ENABLED)
		{
			VIP_SYSTEM_PRIME_AFFECT = parser.getBoolean("PrimeAffectPoints");
			VIP_SYSTEM_L_SHOP_AFFECT = parser.getBoolean("LShopAffectPoints");
			VIP_SYSTEM_MAX_TIER = parser.getInt("MaxVipLevel", 7);
			if (VIP_SYSTEM_MAX_TIER > 10)
			{
				VIP_SYSTEM_MAX_TIER = 10;
			}
		}

		// Load PremiumSystem config file (if exists)
		parser.LoadConfig(CUSTOM_PREMIUM_SYSTEM_CONFIG_FILE);
		PREMIUM_SYSTEM_ENABLED = parser.getBoolean("EnablePremiumSystem");
		PC_CAFE_ENABLED = parser.getBoolean("PcCafeEnabled");
		PC_CAFE_ONLY_PREMIUM = parser.getBoolean("PcCafeOnlyPremium");
		PC_CAFE_ONLY_VIP = parser.getBoolean("PcCafeOnlyVip");
		PC_CAFE_RETAIL_LIKE = parser.getBoolean("PcCafeRetailLike", true);
		PC_CAFE_REWARD_TIME = parser.getInt("PcCafeRewardTime", 300000);
		PC_CAFE_MAX_POINTS = parser.getInt("MaxPcCafePoints", 200000);
		if (PC_CAFE_MAX_POINTS < 0)
		{
			PC_CAFE_MAX_POINTS = 0;
		}

		PC_CAFE_ENABLE_DOUBLE_POINTS = parser.getBoolean("DoublingAcquisitionPoints");
		PC_CAFE_DOUBLE_POINTS_CHANCE = parser.getInt("DoublingAcquisitionPointsChance", 1);
		if ((PC_CAFE_DOUBLE_POINTS_CHANCE < 0) || (PC_CAFE_DOUBLE_POINTS_CHANCE > 100))
		{
			PC_CAFE_DOUBLE_POINTS_CHANCE = 1;
		}

		ACQUISITION_PC_CAFE_RETAIL_LIKE_POINTS = parser.getInt("AcquisitionPointsRetailLikePoints", 10);
		PC_CAFE_POINT_RATE = parser.getDouble("AcquisitionPointsRate", 1.0);
		PC_CAFE_RANDOM_POINT = parser.getBoolean("AcquisitionPointsRandom");
		if (PC_CAFE_POINT_RATE < 0)
		{
			PC_CAFE_POINT_RATE = 1;
		}

		PC_CAFE_REWARD_LOW_EXP_KILLS = parser.getBoolean("RewardLowExpKills", true);
		PC_CAFE_LOW_EXP_KILLS_CHANCE = parser.getInt("RewardLowExpKillsChance", 50);
		if (PC_CAFE_LOW_EXP_KILLS_CHANCE < 0)
		{
			PC_CAFE_LOW_EXP_KILLS_CHANCE = 0;
		}

		if (PC_CAFE_LOW_EXP_KILLS_CHANCE > 100)
		{
			PC_CAFE_LOW_EXP_KILLS_CHANCE = 100;
		}

		PREMIUM_RATE_XP = parser.getFloat("PremiumRateXp", 2);
		PREMIUM_RATE_SP = parser.getFloat("PremiumRateSp", 2);
		PREMIUM_RATE_DROP_CHANCE = parser.getFloat("PremiumRateDropChance", 2);
		PREMIUM_RATE_DROP_AMOUNT = parser.getFloat("PremiumRateDropAmount", 1);
		PREMIUM_RATE_SPOIL_CHANCE = parser.getFloat("PremiumRateSpoilChance", 2);
		PREMIUM_RATE_SPOIL_AMOUNT = parser.getFloat("PremiumRateSpoilAmount", 1);
		PREMIUM_RATE_QUEST_XP = parser.getFloat("PremiumRateQuestXp", 1);
		PREMIUM_RATE_QUEST_SP = parser.getFloat("PremiumRateQuestSp", 1);
		PREMIUM_RATE_DROP_CHANCE_BY_ID = parser.GetIdValueMap<double>("PremiumRateDropChanceByItemId");
		PREMIUM_RATE_DROP_AMOUNT_BY_ID = parser.GetIdValueMap<double>("PremiumRateDropAmountByItemId");
		PREMIUM_ONLY_FISHING = parser.getBoolean("PremiumOnlyFishing", true);

		// Load PrivateStoreRange config file (if exists)
		parser.LoadConfig(CUSTOM_PRIVATE_STORE_RANGE_CONFIG_FILE);
		SHOP_MIN_RANGE_FROM_PLAYER = parser.getInt("ShopMinRangeFromPlayer", 50);
		SHOP_MIN_RANGE_FROM_NPC = parser.getInt("ShopMinRangeFromNpc", 100);

		// Load PvpAnnounce config file (if exists)
		parser.LoadConfig(CUSTOM_PVP_ANNOUNCE_CONFIG_FILE);
		ANNOUNCE_PK_PVP = parser.getBoolean("AnnouncePkPvP");
		ANNOUNCE_PK_PVP_NORMAL_MESSAGE = parser.getBoolean("AnnouncePkPvPNormalMessage", true);
		ANNOUNCE_PK_MSG = parser.getString("AnnouncePkMsg", "$killer has slaughtered $target");
		ANNOUNCE_PVP_MSG = parser.getString("AnnouncePvpMsg", "$killer has defeated $target");

		// Load PvpRewardItem config file (if exists)
		parser.LoadConfig(CUSTOM_PVP_REWARD_ITEM_CONFIG_FILE);
		REWARD_PVP_ITEM = parser.getBoolean("RewardPvpItem");
		REWARD_PVP_ITEM_ID = parser.getInt("RewardPvpItemId", 57);
		REWARD_PVP_ITEM_AMOUNT = parser.getInt("RewardPvpItemAmount", 1000);
		REWARD_PVP_ITEM_MESSAGE = parser.getBoolean("RewardPvpItemMessage", true);
		REWARD_PK_ITEM = parser.getBoolean("RewardPkItem");
		REWARD_PK_ITEM_ID = parser.getInt("RewardPkItemId", 57);
		REWARD_PK_ITEM_AMOUNT = parser.getInt("RewardPkItemAmount", 500);
		REWARD_PK_ITEM_MESSAGE = parser.getBoolean("RewardPkItemMessage", true);
		DISABLE_REWARDS_IN_INSTANCES = parser.getBoolean("DisableRewardsInInstances", true);
		DISABLE_REWARDS_IN_PVP_ZONES = parser.getBoolean("DisableRewardsInPvpZones", true);

		// Load PvpTitle config file (if exists)
		parser.LoadConfig(CUSTOM_PVP_TITLE_CONFIG_FILE);
		PVP_COLOR_SYSTEM_ENABLED = parser.getBoolean("EnablePvPColorSystem");
		PVP_AMOUNT1 = parser.getInt("PvpAmount1", 500);
		PVP_AMOUNT2 = parser.getInt("PvpAmount2", 1000);
		PVP_AMOUNT3 = parser.getInt("PvpAmount3", 1500);
		PVP_AMOUNT4 = parser.getInt("PvpAmount4", 2500);
		PVP_AMOUNT5 = parser.getInt("PvpAmount5", 5000);
		NAME_COLOR_FOR_PVP_AMOUNT1 = parser.GetColor("ColorForAmount1", new Color(0x00FF00));
		NAME_COLOR_FOR_PVP_AMOUNT2 = parser.GetColor("ColorForAmount2", new Color(0x00FF00));
		NAME_COLOR_FOR_PVP_AMOUNT3 = parser.GetColor("ColorForAmount3", new Color(0x00FF00));
		NAME_COLOR_FOR_PVP_AMOUNT4 = parser.GetColor("ColorForAmount4", new Color(0x00FF00));
		NAME_COLOR_FOR_PVP_AMOUNT5 = parser.GetColor("ColorForAmount5", new Color(0x00FF00));
		TITLE_FOR_PVP_AMOUNT1 = parser.getString("PvPTitleForAmount1", "Title");
		TITLE_FOR_PVP_AMOUNT2 = parser.getString("PvPTitleForAmount2", "Title");
		TITLE_FOR_PVP_AMOUNT3 = parser.getString("PvPTitleForAmount3", "Title");
		TITLE_FOR_PVP_AMOUNT4 = parser.getString("PvPTitleForAmount4", "Title");
		TITLE_FOR_PVP_AMOUNT5 = parser.getString("PvPTitleForAmount5", "Title");

		// Load RandomSpawns config file (if exists)
		parser.LoadConfig(CUSTOM_RANDOM_SPAWNS_CONFIG_FILE);
		ENABLE_RANDOM_MONSTER_SPAWNS = parser.getBoolean("EnableRandomMonsterSpawns");
		MOB_MAX_SPAWN_RANGE = parser.getInt("MaxSpawnMobRange", 150);
		MOB_MIN_SPAWN_RANGE = MOB_MAX_SPAWN_RANGE * -1;
		if (ENABLE_RANDOM_MONSTER_SPAWNS)
		{
			MOBS_LIST_NOT_RANDOM = parser.GetIntList("MobsSpawnNotRandom", ',', 18812, 18813, 18814, 22138)
				.ToImmutableSortedSet();
		}

		// Load SayuneForAll config file (if exists)
		parser.LoadConfig(CUSTOM_SAYUNE_FOR_ALL_CONFIG_FILE);
		FREE_JUMPS_FOR_ALL = parser.getBoolean("FreeJumpsForAll");

		// Load ScreenWelcomeMessage config file (if exists)
		parser.LoadConfig(CUSTOM_SCREEN_WELCOME_MESSAGE_CONFIG_FILE);
		WELCOME_MESSAGE_ENABLED = parser.getBoolean("ScreenWelcomeMessageEnable");
		WELCOME_MESSAGE_TEXT = parser.getString("ScreenWelcomeMessageText", "Welcome to our server!");
		WELCOME_MESSAGE_TIME = parser.getInt("ScreenWelcomeMessageTime", 10) * 1000;

		// Load SellBuffs config file (if exists)
		parser.LoadConfig(CUSTOM_SELL_BUFFS_CONFIG_FILE);
		SELLBUFF_ENABLED = parser.getBoolean("SellBuffEnable");
		SELLBUFF_MP_MULTIPLER = parser.getInt("MpCostMultipler", 1);
		SELLBUFF_PAYMENT_ID = parser.getInt("PaymentID", 57);
		SELLBUFF_MIN_PRICE = parser.getLong("MinimumPrice", 100000);
		SELLBUFF_MAX_PRICE = parser.getLong("MaximumPrice", 100000000);
		SELLBUFF_MAX_BUFFS = parser.getInt("MaxBuffs", 15);

		// Load ServerTime config file (if exists)
		parser.LoadConfig(CUSTOM_SERVER_TIME_CONFIG_FILE);
		DISPLAY_SERVER_TIME = parser.getBoolean("DisplayServerTime");

		// Load SchemeBuffer config file (if exists)
		parser.LoadConfig(CUSTOM_SCHEME_BUFFER_CONFIG_FILE);
		BUFFER_MAX_SCHEMES = parser.getInt("BufferMaxSchemesPerChar", 4);
		BUFFER_ITEM_ID = parser.getInt("BufferItemId", 57);
		BUFFER_STATIC_BUFF_COST = parser.getInt("BufferStaticCostPerBuff", -1);

		// Load StartingLocation config file (if exists)
		parser.LoadConfig(CUSTOM_STARTING_LOCATION_CONFIG_FILE);
		CUSTOM_STARTING_LOC = parser.getBoolean("CustomStartingLocation");
		CUSTOM_STARTING_LOC_X = parser.getInt("CustomStartingLocX", 50821);
		CUSTOM_STARTING_LOC_Y = parser.getInt("CustomStartingLocY", 186527);
		CUSTOM_STARTING_LOC_Z = parser.getInt("CustomStartingLocZ", -3625);

		// Load WalkerBotProtection config file (if exists)
		parser.LoadConfig(CUSTOM_WALKER_BOT_PROTECTION_CONFIG_FILE);
		L2WALKER_PROTECTION = parser.getBoolean("L2WalkerProtection");
	}

	/**
	 * Loads flood protector configurations.
	 * @param properties the properties object containing the actual values of the flood protector configs
	 */
	private static void LoadFloodProtectorConfigs(ConfigurationParser parser)
	{
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_USE_ITEM, "UseItem", 4);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_ROLL_DICE, "RollDice", 42);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_ITEM_PET_SUMMON, "ItemPetSummon", 16);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_HERO_VOICE, "HeroVoice", 100);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_GLOBAL_CHAT, "GlobalChat", 5);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_SUBCLASS, "Subclass", 20);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_DROP_ITEM, "DropItem", 10);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_SERVER_BYPASS, "ServerBypass", 5);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_MULTISELL, "MultiSell", 1);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_TRANSACTION, "Transaction", 10);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_MANUFACTURE, "Manufacture", 3);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_SENDMAIL, "SendMail", 100);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_CHARACTER_SELECT, "CharacterSelect", 30);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_ITEM_AUCTION, "ItemAuction", 9);
		LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_PLAYER_ACTION, "PlayerAction", 3);
	}

	/**
	 * Loads single flood protector configuration.
	 * @param properties properties file reader
	 * @param config flood protector configuration instance
	 * @param configString flood protector configuration string that determines for which flood protector configuration should be read
	 * @param defaultInterval default flood protector interval
	 */
	private static void LoadFloodProtectorConfig(ConfigurationParser parser, FloodProtectorConfig config,
		string configString, int defaultInterval)
	{
		config.FLOOD_PROTECTION_INTERVAL = parser.getInt("FloodProtector" + configString + "Interval", defaultInterval);
		config.LOG_FLOODING = parser.getBoolean("FloodProtector" + configString + "LogFlooding");
		config.PUNISHMENT_LIMIT = parser.getInt("FloodProtector" + configString + "PunishmentLimit");
		config.PUNISHMENT_TYPE = parser.getString("FloodProtector" + configString + "PunishmentType", "none");
		config.PUNISHMENT_TIME = parser.getInt("FloodProtector" + configString + "PunishmentTime") * 60000;
	}

	private static ImmutableArray<DropHolder> GetDropList(ConfigurationParser parser, string key)
	{
		// Format:
		// itemId1,minAmount1,maxAmount1,chance1;itemId2...

		return parser.GetList(key, ';', s =>
		{
			string[] item = s.Split(',');
			int itemId = 0;
			int min = 0;
			int max = 0;
			double rate = 0;
			bool ok = item.Length == 4 && int.TryParse(item[0], CultureInfo.InvariantCulture, out itemId) &&
			          int.TryParse(item[1], CultureInfo.InvariantCulture, out min) &&
			          int.TryParse(item[2], CultureInfo.InvariantCulture, out max) &&
			          double.TryParse(item[3], CultureInfo.InvariantCulture, out rate);

			return (new DropHolder(DropType.DROP, itemId, min, max, rate), ok);
		}, true).ToImmutableArray();
	}

	private static ImmutableDictionary<CharacterClass, double> GetMultipliers(ConfigurationParser parser, string key)
	{
		// Format:
		// ELVEN_FIGHTER*2;PALUS_KNIGHT*2.5;...

		var builder = ImmutableDictionary<CharacterClass, double>.Empty.ToBuilder();
		parser.GetList(key, ';', s =>
		{
			string[] item = s.Split('*');
			CharacterClass classId;
			bool ok = double.TryParse(item[1], CultureInfo.InvariantCulture, out double rate);
			if (int.TryParse(item[0], CultureInfo.InvariantCulture, out int classNum))
				classId = (CharacterClass)classNum;
			else if (!Enum.TryParse(item[0], false, out classId))
				ok = false;

			return ((classId, rate), ok);
		}, true).ForEach(tuple =>
		{
			try
			{
				builder.Add(tuple.classId, tuple.rate);
			}
			catch (ArgumentException)
			{
				LOGGER.Error($"Duplicated class '{tuple.classId}' in entry '{key}' in configuration file '{parser.FilePath}'");
			}

		});

		return builder.ToImmutable();
	}

	private static Location GetLocation(ConfigurationParser parser, string key, int dx, int dy, int dz,
		int dheading = 0)
	{
		string value = parser.getString(key);
		if (string.IsNullOrEmpty(value))
			return new Location(dx, dy, dz, dheading);

		string[] k = value.Split(',');
		if ((k.Length == 3 || k.Length == 4) && int.TryParse(k[0], CultureInfo.InvariantCulture, out int x) &&
		    int.TryParse(k[1], CultureInfo.InvariantCulture, out int y) &&
		    int.TryParse(k[2], CultureInfo.InvariantCulture, out int z))
		{
			if (k.Length == 4)
			{
				if (int.TryParse(k[3], CultureInfo.InvariantCulture, out int heading))
				{
					return new Location(x, y, z, heading);
				}
			}
			else
			{
				return new Location(x, y, z, dheading);
			}
		}

		LOGGER.Error($"Invalid location format '{value}' in entry '{key}' in configuration file '{parser.FilePath}'");

		return new Location(dx, dy, dz, dheading);
	}

	private static ImmutableDictionary<string, Location> GetLocations(ConfigurationParser parser, string key)
	{
		// Format:
		// TeleportName1,X1,Y1,Z1;TeleportName2,X2,Y2,Z2...

		ImmutableDictionary<string, Location>.Builder builder =
			ImmutableDictionary<string, Location>.Empty.ToBuilder();

		parser.GetList(key, ';', s =>
		{
			string[] item = s.Split(',');
			int x;
			int y = 0;
			int z = 0;
			bool ok = int.TryParse(item[1], CultureInfo.InvariantCulture, out x) &&
			          int.TryParse(item[2], CultureInfo.InvariantCulture, out y) &&
			          int.TryParse(item[3], CultureInfo.InvariantCulture, out z);
			return ((Name: item[0], Location: new Location(x, y, z, 0)), ok);
		}, true).ForEach(tuple =>
		{
			try
			{
				builder.Add(tuple.Name, tuple.Location);
			}
			catch (ArgumentException)
			{
				LOGGER.Error(
					$"Duplicated location name '{tuple.Name}' in entry '{key}' in configuration file '{parser.FilePath}'");
			}

		});

		return builder.ToImmutable();
	}
}