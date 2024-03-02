using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using L2Dn.Configuration;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer;

/**
 * This class loads all the game server related configurations from files.<br>
 * The files are usually located in config folder in server root folder.<br>
 * Each configuration has a default value (that should reflect retail behavior).
 */
public class Config
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Config));

	// --------------------------------------------------
	// Config File Definitions
	// --------------------------------------------------
	public const string INTERFACE_CONFIG_FILE = "./config/Interface.ini";
	public const string NETWORK_CONFIG_FILE = "./config/Network.ini";
	public const string OLYMPIAD_CONFIG_FILE = "./config/Olympiad.ini";

	public const string FORTSIEGE_CONFIG_FILE = "./config/FortSiege.ini";
	public const string SIEGE_CONFIG_FILE = "./config/Siege.ini";

	private const string ATTENDANCE_CONFIG_FILE = "./config/AttendanceRewards.ini";
	private const string ATTRIBUTE_SYSTEM_FILE = "./config/AttributeSystem.ini";
	private const string CHARACTER_CONFIG_FILE = "./config/Character.ini";
	private const string FEATURE_CONFIG_FILE = "./config/Feature.ini";
	private const string FLOOD_PROTECTOR_CONFIG_FILE = "./config/FloodProtector.ini";
	private const string GAME_ASSISTANT_CONFIG_FILE = "./config/GameAssistant.ini";
	private const string GENERAL_CONFIG_FILE = "./config/General.ini";
	private const string GEOENGINE_CONFIG_FILE = "./config/GeoEngine.ini";
	private const string GRACIASEEDS_CONFIG_FILE = "./config/GraciaSeeds.ini";
	private const string GRANDBOSS_CONFIG_FILE = "./config/GrandBoss.ini";
	private const string HUNT_PASS_CONFIG_FILE = "./config/HuntPass.ini";
	private const string ACHIEVEMENT_BOX_CONFIG_FILE = "./config/AchievementBox.ini";
	private const string LOGIN_CONFIG_FILE = "./config/LoginServer.ini";
	private const string MAGIC_LAMP_FILE = "./config/MagicLamp.ini";
	private const string NPC_CONFIG_FILE = "./config/NPC.ini";
	private const string ORC_FORTRESS_CONFIG_FILE = "./config/OrcFortress.ini";
	private const string PVP_CONFIG_FILE = "./config/PVP.ini";
	private const string RANDOM_CRAFT_FILE = "./config/RandomCraft.ini";
	private const string RATES_CONFIG_FILE = "./config/Rates.ini";
	private const string SERVER_CONFIG_FILE = "./config/Server.ini";
	private const string TRAINING_CAMP_CONFIG_FILE = "./config/TrainingCamp.ini";
	private const string WORLD_EXCHANGE_FILE = "./config/WorldExchange.ini";

	private const string CHAT_FILTER_FILE = "./config/chatfilter.txt";

	// --------------------------------------------------
	// Custom Config File Definitions
	// --------------------------------------------------
	private const string CUSTOM_ALLOWED_PLAYER_RACES_CONFIG_FILE = "./config/Custom/AllowedPlayerRaces.ini";
	private const string CUSTOM_AUTO_POTIONS_CONFIG_FILE = "./config/Custom/AutoPotions.ini";
	private const string CUSTOM_BANKING_CONFIG_FILE = "./config/Custom/Banking.ini";
	private const string CUSTOM_BOSS_ANNOUNCEMENTS_CONFIG_FILE = "./config/Custom/BossAnnouncements.ini";
	private const string CUSTOM_CHAMPION_MONSTERS_CONFIG_FILE = "./config/Custom/ChampionMonsters.ini";
	private const string CUSTOM_CHAT_MODERATION_CONFIG_FILE = "./config/Custom/ChatModeration.ini";
	private const string CUSTOM_CLASS_BALANCE_CONFIG_FILE = "./config/Custom/ClassBalance.ini";
	private const string CUSTOM_COMMUNITY_BOARD_CONFIG_FILE = "./config/Custom/CommunityBoard.ini";
	private const string CUSTOM_CUSTOM_DEPOSITABLE_ITEMS_CONFIG_FILE = "./config/Custom/CustomDepositableItems.ini";
	private const string CUSTOM_CUSTOM_MAIL_MANAGER_CONFIG_FILE = "./config/Custom/CustomMailManager.ini";
	private const string CUSTOM_DELEVEL_MANAGER_CONFIG_FILE = "./config/Custom/DelevelManager.ini";
	private const string CUSTOM_DUALBOX_CHECK_CONFIG_FILE = "./config/Custom/DualboxCheck.ini";
	private const string CUSTOM_FACTION_SYSTEM_CONFIG_FILE = "./config/Custom/FactionSystem.ini";
	private const string CUSTOM_FAKE_PLAYERS_CONFIG_FILE = "./config/Custom/FakePlayers.ini";
	private const string CUSTOM_FIND_PVP_CONFIG_FILE = "./config/Custom/FindPvP.ini";
	private const string CUSTOM_MERCHANT_ZERO_SELL_PRICE_CONFIG_FILE = "./config/Custom/MerchantZeroSellPrice.ini";
	private const string CUSTOM_MULTILANGUAL_SUPPORT_CONFIG_FILE = "./config/Custom/MultilingualSupport.ini";
	private const string CUSTOM_NOBLESS_MASTER_CONFIG_FILE = "./config/Custom/NoblessMaster.ini";
	private const string CUSTOM_NPC_STAT_MULTIPLIERS_CONFIG_FILE = "./config/Custom/NpcStatMultipliers.ini";
	private const string CUSTOM_OFFLINE_PLAY_CONFIG_FILE = "./config/Custom/OfflinePlay.ini";
	private const string CUSTOM_OFFLINE_TRADE_CONFIG_FILE = "./config/Custom/OfflineTrade.ini";
	private const string CUSTOM_ONLINE_INFO_CONFIG_FILE = "./config/Custom/OnlineInfo.ini";
	private const string CUSTOM_PASSWORD_CHANGE_CONFIG_FILE = "./config/Custom/PasswordChange.ini";
	private const string CUSTOM_VIP_CONFIG_FILE = "./config/Custom/VipSystem.ini";
	private const string CUSTOM_PREMIUM_SYSTEM_CONFIG_FILE = "./config/Custom/PremiumSystem.ini";
	private const string CUSTOM_PRIVATE_STORE_RANGE_CONFIG_FILE = "./config/Custom/PrivateStoreRange.ini";
	private const string CUSTOM_PVP_ANNOUNCE_CONFIG_FILE = "./config/Custom/PvpAnnounce.ini";
	private const string CUSTOM_PVP_REWARD_ITEM_CONFIG_FILE = "./config/Custom/PvpRewardItem.ini";
	private const string CUSTOM_PVP_TITLE_CONFIG_FILE = "./config/Custom/PvpTitleColor.ini";
	private const string CUSTOM_RANDOM_SPAWNS_CONFIG_FILE = "./config/Custom/RandomSpawns.ini";
	private const string CUSTOM_SAYUNE_FOR_ALL_CONFIG_FILE = "./config/Custom/SayuneForAll.ini";
	private const string CUSTOM_SCREEN_WELCOME_MESSAGE_CONFIG_FILE = "./config/Custom/ScreenWelcomeMessage.ini";
	private const string CUSTOM_SELL_BUFFS_CONFIG_FILE = "./config/Custom/SellBuffs.ini";
	private const string CUSTOM_SERVER_TIME_CONFIG_FILE = "./config/Custom/ServerTime.ini";
	private const string CUSTOM_SCHEME_BUFFER_CONFIG_FILE = "./config/Custom/SchemeBuffer.ini";
	private const string CUSTOM_STARTING_LOCATION_CONFIG_FILE = "./config/Custom/StartingLocation.ini";
	private const string CUSTOM_WALKER_BOT_PROTECTION_CONFIG_FILE = "./config/Custom/WalkerBotProtection.ini";

	// --------------------------------------------------
	// Variable Definitions
	// --------------------------------------------------
	public static bool ENABLE_ATTENDANCE_REWARDS;
	public static bool PREMIUM_ONLY_ATTENDANCE_REWARDS;
	public static bool VIP_ONLY_ATTENDANCE_REWARDS;
	public static bool ATTENDANCE_REWARDS_SHARE_ACCOUNT;
	public static int ATTENDANCE_REWARD_DELAY;
	public static bool ATTENDANCE_POPUP_START;
	public static bool ATTENDANCE_POPUP_WINDOW;
	public static bool PLAYER_DELEVEL;
	public static int DELEVEL_MINIMUM;
	public static bool DECREASE_SKILL_LEVEL;
	public static double ALT_WEIGHT_LIMIT;
	public static int RUN_SPD_BOOST;
	public static double RESPAWN_RESTORE_CP;
	public static double RESPAWN_RESTORE_HP;
	public static double RESPAWN_RESTORE_MP;
	public static bool ENABLE_MODIFY_SKILL_DURATION;
	public static ImmutableDictionary<int, TimeSpan> SKILL_DURATION_LIST = ImmutableDictionary<int, TimeSpan>.Empty;
	public static bool ENABLE_MODIFY_SKILL_REUSE;
	public static ImmutableDictionary<int, TimeSpan> SKILL_REUSE_LIST = ImmutableDictionary<int, TimeSpan>.Empty;
	public static bool AUTO_LEARN_SKILLS;
	public static bool AUTO_LEARN_SKILLS_WITHOUT_ITEMS;
	public static bool AUTO_LEARN_FS_SKILLS;
	public static bool AUTO_LOOT_HERBS;
	public static byte BUFFS_MAX_AMOUNT;
	public static byte TRIGGERED_BUFFS_MAX_AMOUNT;
	public static byte DANCES_MAX_AMOUNT;
	public static bool DANCE_CANCEL_BUFF;
	public static bool DANCE_CONSUME_ADDITIONAL_MP;
	public static bool ALT_STORE_DANCES;
	public static bool AUTO_LEARN_DIVINE_INSPIRATION;
	public static bool ALT_GAME_CANCEL_BOW;
	public static bool ALT_GAME_CANCEL_CAST;
	public static bool ALT_GAME_MAGICFAILURES;
	public static bool ALT_GAME_STUN_BREAK;
	public static int PLAYER_FAKEDEATH_UP_PROTECTION;
	public static bool STORE_SKILL_COOLTIME;
	public static bool SUBCLASS_STORE_SKILL_COOLTIME;
	public static bool SUMMON_STORE_SKILL_COOLTIME;
	public static long EFFECT_TICK_RATIO;
	public static bool FAKE_DEATH_UNTARGET;
	public static bool FAKE_DEATH_DAMAGE_STAND;
	public static bool VAMPIRIC_ATTACK_WORKS_WITH_SKILLS;
	public static bool MP_VAMPIRIC_ATTACK_WORKS_WITH_MELEE;
	public static bool CALCULATE_MAGIC_SUCCESS_BY_SKILL_MAGIC_LEVEL;
	public static int BLOW_RATE_CHANCE_LIMIT;
	public static int ITEM_EQUIP_ACTIVE_SKILL_REUSE;
	public static int ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE;
	public static double PLAYER_REFLECT_PERCENT_LIMIT;
	public static double NON_PLAYER_REFLECT_PERCENT_LIMIT;
	public static bool LIFE_CRYSTAL_NEEDED;
	public static bool DIVINE_SP_BOOK_NEEDED;
	public static bool ALT_GAME_SUBCLASS_WITHOUT_QUESTS;
	public static bool ALT_GAME_SUBCLASS_EVERYWHERE;
	public static bool ALLOW_TRANSFORM_WITHOUT_QUEST;
	public static int FEE_DELETE_TRANSFER_SKILLS;
	public static int FEE_DELETE_SUBCLASS_SKILLS;
	public static int FEE_DELETE_DUALCLASS_SKILLS;
	public static bool RESTORE_SERVITOR_ON_RECONNECT;
	public static bool RESTORE_PET_ON_RECONNECT;
	public static double MAX_BONUS_EXP;
	public static double MAX_BONUS_SP;
	public static int MAX_RUN_SPEED;
	public static int MAX_RUN_SPEED_SUMMON;
	public static int MAX_PATK;
	public static int MAX_MATK;
	public static int MAX_PCRIT_RATE;
	public static int MAX_MCRIT_RATE;
	public static int MAX_PATK_SPEED;
	public static int MAX_MATK_SPEED;
	public static int MAX_EVASION;
	public static int MAX_HP;
	public static int MIN_ABNORMAL_STATE_SUCCESS_RATE;
	public static int MAX_ABNORMAL_STATE_SUCCESS_RATE;
	public static long MAX_SP;
	public static int PLAYER_MAXIMUM_LEVEL;
	public static int MAX_SUBCLASS;
	public static int BASE_SUBCLASS_LEVEL;
	public static int BASE_DUALCLASS_LEVEL;
	public static int MAX_SUBCLASS_LEVEL;
	public static int MAX_PVTSTORESELL_SLOTS_DWARF;
	public static int MAX_PVTSTORESELL_SLOTS_OTHER;
	public static int MAX_PVTSTOREBUY_SLOTS_DWARF;
	public static int MAX_PVTSTOREBUY_SLOTS_OTHER;
	public static int INVENTORY_MAXIMUM_NO_DWARF;
	public static int INVENTORY_MAXIMUM_DWARF;
	public static int INVENTORY_MAXIMUM_GM;
	public static int INVENTORY_MAXIMUM_QUEST_ITEMS;
	public static int WAREHOUSE_SLOTS_DWARF;
	public static int WAREHOUSE_SLOTS_NO_DWARF;
	public static int WAREHOUSE_SLOTS_CLAN;
	public static int ALT_FREIGHT_SLOTS;
	public static int ALT_FREIGHT_PRICE;
	public static long MENTOR_PENALTY_FOR_MENTEE_COMPLETE;
	public static long MENTOR_PENALTY_FOR_MENTEE_LEAVE;
	public static bool ALT_GAME_KARMA_PLAYER_CAN_BE_KILLED_IN_PEACEZONE;
	public static bool ALT_GAME_KARMA_PLAYER_CAN_SHOP;
	public static bool ALT_GAME_KARMA_PLAYER_CAN_TELEPORT;
	public static bool ALT_GAME_KARMA_PLAYER_CAN_USE_GK;
	public static bool ALT_GAME_KARMA_PLAYER_CAN_TRADE;
	public static bool ALT_GAME_KARMA_PLAYER_CAN_USE_WAREHOUSE;
	public static int MAX_PERSONAL_FAME_POINTS;
	public static int FORTRESS_ZONE_FAME_TASK_FREQUENCY;
	public static int FORTRESS_ZONE_FAME_AQUIRE_POINTS;
	public static int CASTLE_ZONE_FAME_TASK_FREQUENCY;
	public static int CASTLE_ZONE_FAME_AQUIRE_POINTS;
	public static bool FAME_FOR_DEAD_PLAYERS;
	public static bool IS_CRAFTING_ENABLED;
	public static bool CRAFT_MASTERWORK;
	public static int DWARF_RECIPE_LIMIT;
	public static int COMMON_RECIPE_LIMIT;
	public static bool ALT_GAME_CREATION;
	public static double ALT_GAME_CREATION_SPEED;
	public static double ALT_GAME_CREATION_XP_RATE;
	public static double ALT_GAME_CREATION_RARE_XPSP_RATE;
	public static double ALT_GAME_CREATION_SP_RATE;
	public static bool ALT_CLAN_LEADER_INSTANT_ACTIVATION;
	public static int ALT_CLAN_JOIN_MINS;
	public static int ALT_CLAN_CREATE_DAYS;
	public static int ALT_CLAN_DISSOLVE_DAYS;
	public static int ALT_ALLY_JOIN_DAYS_WHEN_LEAVED;
	public static int ALT_ALLY_JOIN_DAYS_WHEN_DISMISSED;
	public static int ALT_ACCEPT_CLAN_DAYS_WHEN_DISMISSED;
	public static int ALT_CREATE_ALLY_DAYS_WHEN_DISSOLVED;
	public static int ALT_MAX_NUM_OF_CLANS_IN_ALLY;
	public static int ALT_CLAN_MEMBERS_FOR_WAR;
	public static bool ALT_MEMBERS_CAN_WITHDRAW_FROM_CLANWH;
	public static TimeSpan ALT_CLAN_MEMBERS_TIME_FOR_BONUS;
	public static bool REMOVE_CASTLE_CIRCLETS;
	public static int ALT_PARTY_MAX_MEMBERS;
	public static int ALT_PARTY_RANGE;
	public static bool ALT_LEAVE_PARTY_LEADER;
	public static bool ALT_COMMAND_CHANNEL_FRIENDS;
	public static bool INITIAL_EQUIPMENT_EVENT;
	public static long STARTING_ADENA;
	public static int STARTING_LEVEL;
	public static int STARTING_SP;
	public static long MAX_ADENA;
	public static bool AUTO_LOOT;
	public static bool AUTO_LOOT_RAIDS;
	public static bool AUTO_LOOT_SLOT_LIMIT;
	public static int LOOT_RAIDS_PRIVILEGE_INTERVAL;
	public static int LOOT_RAIDS_PRIVILEGE_CC_SIZE;
	public static ImmutableSortedSet<int> AUTO_LOOT_ITEM_IDS = ImmutableSortedSet<int>.Empty;
	public static bool ENABLE_KEYBOARD_MOVEMENT;
	public static int UNSTUCK_INTERVAL;
	public static int TELEPORT_WATCHDOG_TIMEOUT;
	public static int PLAYER_SPAWN_PROTECTION;
	public static int PLAYER_TELEPORT_PROTECTION;
	public static bool RANDOM_RESPAWN_IN_TOWN_ENABLED;
	public static bool OFFSET_ON_TELEPORT_ENABLED;
	public static int MAX_OFFSET_ON_TELEPORT;
	public static bool TELEPORT_WHILE_SIEGE_IN_PROGRESS;
	public static bool TELEPORT_WHILE_PLAYER_IN_COMBAT;
	public static bool PETITIONING_ALLOWED;
	public static int MAX_PETITIONS_PER_PLAYER;
	public static int MAX_PETITIONS_PENDING;
	public static int MAX_FREE_TELEPORT_LEVEL;
	public static int MAX_NEWBIE_BUFF_LEVEL;
	public static int DELETE_DAYS;
	public static bool DISCONNECT_AFTER_DEATH;
	public static string PARTY_XP_CUTOFF_METHOD;
	public static double PARTY_XP_CUTOFF_PERCENT;
	public static int PARTY_XP_CUTOFF_LEVEL;
	public static ImmutableArray<Range<int>> PARTY_XP_CUTOFF_GAPS;
	public static ImmutableArray<int> PARTY_XP_CUTOFF_GAP_PERCENTS = ImmutableArray<int>.Empty;
	public static bool DISABLE_TUTORIAL;
	public static bool STORE_RECIPE_SHOPLIST;
	public static bool STORE_UI_SETTINGS;
	public static ImmutableSortedSet<string> FORBIDDEN_NAMES = ImmutableSortedSet<string>.Empty;
	public static bool SILENCE_MODE_EXCLUDE;

	// --------------------------------------------------
	// Castle Settings
	// --------------------------------------------------
	public static long CS_TELE_FEE_RATIO;
	public static int CS_TELE1_FEE;
	public static int CS_TELE2_FEE;
	public static long CS_MPREG_FEE_RATIO;
	public static int CS_MPREG1_FEE;
	public static int CS_MPREG2_FEE;
	public static long CS_HPREG_FEE_RATIO;
	public static int CS_HPREG1_FEE;
	public static int CS_HPREG2_FEE;
	public static long CS_EXPREG_FEE_RATIO;
	public static int CS_EXPREG1_FEE;
	public static int CS_EXPREG2_FEE;
	public static long CS_SUPPORT_FEE_RATIO;
	public static int CS_SUPPORT1_FEE;
	public static int CS_SUPPORT2_FEE;
	public static ImmutableArray<int> SIEGE_HOUR_LIST = ImmutableArray<int>.Empty;
	public static int CASTLE_BUY_TAX_NEUTRAL;
	public static int CASTLE_BUY_TAX_LIGHT;
	public static int CASTLE_BUY_TAX_DARK;
	public static int CASTLE_SELL_TAX_NEUTRAL;
	public static int CASTLE_SELL_TAX_LIGHT;
	public static int CASTLE_SELL_TAX_DARK;
	public static int OUTER_DOOR_UPGRADE_PRICE2;
	public static int OUTER_DOOR_UPGRADE_PRICE3;
	public static int OUTER_DOOR_UPGRADE_PRICE5;
	public static int INNER_DOOR_UPGRADE_PRICE2;
	public static int INNER_DOOR_UPGRADE_PRICE3;
	public static int INNER_DOOR_UPGRADE_PRICE5;
	public static int WALL_UPGRADE_PRICE2;
	public static int WALL_UPGRADE_PRICE3;
	public static int WALL_UPGRADE_PRICE5;
	public static int TRAP_UPGRADE_PRICE1;
	public static int TRAP_UPGRADE_PRICE2;
	public static int TRAP_UPGRADE_PRICE3;
	public static int TRAP_UPGRADE_PRICE4;

	// --------------------------------------------------
	// Fortress Settings
	// --------------------------------------------------
	public static TimeSpan FS_TELE_FEE_RATIO;
	public static int FS_TELE1_FEE;
	public static int FS_TELE2_FEE;
	public static TimeSpan FS_MPREG_FEE_RATIO;
	public static int FS_MPREG1_FEE;
	public static int FS_MPREG2_FEE;
	public static TimeSpan FS_HPREG_FEE_RATIO;
	public static int FS_HPREG1_FEE;
	public static int FS_HPREG2_FEE;
	public static TimeSpan FS_EXPREG_FEE_RATIO;
	public static int FS_EXPREG1_FEE;
	public static int FS_EXPREG2_FEE;
	public static TimeSpan FS_SUPPORT_FEE_RATIO;
	public static int FS_SUPPORT1_FEE;
	public static int FS_SUPPORT2_FEE;
	public static int FS_BLOOD_OATH_COUNT;
	public static int FS_UPDATE_FRQ;
	public static int FS_MAX_SUPPLY_LEVEL;
	public static int FS_FEE_FOR_CASTLE;
	public static int FS_MAX_OWN_TIME;

	public static bool ORC_FORTRESS_ENABLE;
	public static TimeOnly ORC_FORTRESS_TIME;

	// --------------------------------------------------
	// Feature Settings
	// --------------------------------------------------
	public static int TAKE_FORT_POINTS;
	public static int LOOSE_FORT_POINTS;
	public static int TAKE_CASTLE_POINTS;
	public static int LOOSE_CASTLE_POINTS;
	public static int CASTLE_DEFENDED_POINTS;
	public static int FESTIVAL_WIN_POINTS;
	public static int HERO_POINTS;
	public static int ROYAL_GUARD_COST;
	public static int KNIGHT_UNIT_COST;
	public static int KNIGHT_REINFORCE_COST;
	public static int BALLISTA_POINTS;
	public static int BLOODALLIANCE_POINTS;
	public static int BLOODOATH_POINTS;
	public static int KNIGHTSEPAULETTE_POINTS;
	public static int REPUTATION_SCORE_PER_KILL;
	public static int JOIN_ACADEMY_MIN_REP_SCORE;
	public static int JOIN_ACADEMY_MAX_REP_SCORE;
	public static int LVL_UP_20_AND_25_REP_SCORE;
	public static int LVL_UP_26_AND_30_REP_SCORE;
	public static int LVL_UP_31_AND_35_REP_SCORE;
	public static int LVL_UP_36_AND_40_REP_SCORE;
	public static int LVL_UP_41_AND_45_REP_SCORE;
	public static int LVL_UP_46_AND_50_REP_SCORE;
	public static int LVL_UP_51_AND_55_REP_SCORE;
	public static int LVL_UP_56_AND_60_REP_SCORE;
	public static int LVL_UP_61_AND_65_REP_SCORE;
	public static int LVL_UP_66_AND_70_REP_SCORE;
	public static int LVL_UP_71_AND_75_REP_SCORE;
	public static int LVL_UP_76_AND_80_REP_SCORE;
	public static int LVL_UP_81_AND_90_REP_SCORE;
	public static int LVL_UP_91_PLUS_REP_SCORE;
	public static double LVL_OBTAINED_REP_SCORE_MULTIPLIER;
	public static int CLAN_LEVEL_6_COST;
	public static int CLAN_LEVEL_7_COST;
	public static int CLAN_LEVEL_8_COST;
	public static int CLAN_LEVEL_9_COST;
	public static int CLAN_LEVEL_10_COST;
	public static int CLAN_LEVEL_6_REQUIREMENT;
	public static int CLAN_LEVEL_7_REQUIREMENT;
	public static int CLAN_LEVEL_8_REQUIREMENT;
	public static int CLAN_LEVEL_9_REQUIREMENT;
	public static int CLAN_LEVEL_10_REQUIREMENT;
	public static bool ALLOW_WYVERN_ALWAYS;
	public static bool ALLOW_WYVERN_DURING_SIEGE;
	public static bool ALLOW_MOUNTS_DURING_SIEGE;

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
	public static int SCHEDULED_THREAD_POOL_SIZE;
	public static int INSTANT_THREAD_POOL_SIZE;
	public static bool THREADS_FOR_LOADING;
	public static bool DEADLOCK_DETECTOR;
	public static int DEADLOCK_CHECK_INTERVAL;
	public static bool RESTART_ON_DEADLOCK;
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
	public static string DEFAULT_GLOBAL_CHAT;
	public static string DEFAULT_TRADE_CHAT;
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
	public static string BBS_DEFAULT;
	public static bool USE_SAY_FILTER;
	public static string CHAT_FILTER_CHARS;
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
	public static string ALT_OLY_PERIOD;
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
	public static string ALT_BIRTHDAY_MAIL_SUBJECT;
	public static string ALT_BIRTHDAY_MAIL_TEXT;
	public static int PLAYER_MOVEMENT_BLOCK_TIME;
	public static int ABILITY_MAX_POINTS;
	public static long ABILITY_POINTS_RESET_ADENA;
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
	public static int CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY;
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
	// Server Settings
	// --------------------------------------------------
	public static int PORT_GAME;
	public static int PORT_LOGIN;
	public static string LOGIN_BIND_ADDRESS;
	public static int LOGIN_TRY_BEFORE_BAN;
	public static int LOGIN_BLOCK_AFTER_BAN;
	public static string GAMESERVER_HOSTNAME;
	public static string DATABASE_DRIVER;
	public static string DATABASE_URL;
	public static string DATABASE_LOGIN;
	public static string DATABASE_PASSWORD;
	public static int DATABASE_MAX_CONNECTIONS;
	public static bool BACKUP_DATABASE;
	public static string MYSQL_BIN_PATH;
	public static string BACKUP_PATH;
	public static int BACKUP_DAYS;
	public static int MAXIMUM_ONLINE_USERS;
	public static bool HARDWARE_INFO_ENABLED;
	public static bool KICK_MISSING_HWID;
	public static int MAX_PLAYERS_PER_HWID;
	public static Regex CHARNAME_TEMPLATE_PATTERN;
	public static Regex PET_NAME_TEMPLATE;
	public static Regex CLAN_NAME_TEMPLATE;
	public static int MAX_CHARACTERS_NUMBER_PER_ACCOUNT;
	public static string DATAPACK_ROOT_PATH;
	public static string SCRIPT_ROOT_PATH;
	public static bool ACCEPT_ALTERNATE_ID;
	public static int REQUEST_ID;
	public static bool RESERVE_HOST_ON_LOGIN = false;
	public static ImmutableArray<int> PROTOCOL_LIST = ImmutableArray<int>.Empty;
	public static ServerType SERVER_LIST_TYPE;
	public static int SERVER_LIST_AGE;
	public static bool SERVER_LIST_BRACKET;
	public static bool LOGIN_SERVER_SCHEDULE_RESTART;
	public static long LOGIN_SERVER_SCHEDULE_RESTART_TIME;
	public static bool SERVER_RESTART_SCHEDULE_ENABLED;
	public static bool SERVER_RESTART_SCHEDULE_MESSAGE;
	public static int SERVER_RESTART_SCHEDULE_COUNTDOWN;
	public static ImmutableArray<TimeOnly> SERVER_RESTART_SCHEDULE = ImmutableArray<TimeOnly>.Empty;
	public static ImmutableArray<DayOfWeek> SERVER_RESTART_DAYS = ImmutableArray<DayOfWeek>.Empty;
	public static bool PRECAUTIONARY_RESTART_ENABLED;
	public static bool PRECAUTIONARY_RESTART_CPU;
	public static bool PRECAUTIONARY_RESTART_MEMORY;
	public static bool PRECAUTIONARY_RESTART_CHECKS;
	public static int PRECAUTIONARY_RESTART_PERCENTAGE;
	public static int PRECAUTIONARY_RESTART_DELAY;

	// --------------------------------------------------
	// Network Settings
	// --------------------------------------------------
	public static int CLIENT_READ_POOL_SIZE;
	public static int CLIENT_SEND_POOL_SIZE;
	public static int CLIENT_EXECUTE_POOL_SIZE;
	public static int PACKET_QUEUE_LIMIT;
	public static bool PACKET_FLOOD_DISCONNECT;
	public static bool PACKET_FLOOD_DROP;
	public static bool PACKET_FLOOD_LOGGED;
	public static bool PACKET_ENCRYPTION;
	public static bool FAILED_DECRYPTION_LOGGED;
	public static bool TCP_NO_DELAY;

	// --------------------------------------------------
	// Vitality Settings
	// --------------------------------------------------
	public static bool ENABLE_VITALITY;
	public static int STARTING_VITALITY_POINTS;
	public static bool RAIDBOSS_USE_VITALITY;
	public static float RATE_VITALITY_EXP_MULTIPLIER;
	public static float RATE_LIMITED_SAYHA_GRACE_EXP_MULTIPLIER;
	public static int VITALITY_MAX_ITEMS_ALLOWED;
	public static float RATE_VITALITY_LOST;
	public static float RATE_VITALITY_GAIN;

	// --------------------------------------------------
	// No classification assigned to the following yet
	// --------------------------------------------------
	public static int MAX_ITEM_IN_PACKET;
	public static int GAME_SERVER_LOGIN_PORT;
	public static string GAME_SERVER_LOGIN_HOST;
	public static List<string> GAME_SERVER_SUBNETS;
	public static List<string> GAME_SERVER_HOSTS;
	public static int PVP_NORMAL_TIME;
	public static int PVP_PVP_TIME;
	public static int MAX_REPUTATION;
	public static int REPUTATION_INCREASE;

	public static ImmutableSortedSet<int> ENCHANT_BLACKLIST = ImmutableSortedSet<int>.Empty;
	public static bool DISABLE_OVER_ENCHANTING;
	public static bool OVER_ENCHANT_PROTECTION;
	public static IllegalActionPunishmentType OVER_ENCHANT_PUNISHMENT;
	public static int MIN_ARMOR_ENCHANT_ANNOUNCE;
	public static int MIN_WEAPON_ENCHANT_ANNOUNCE;
	public static int MAX_ARMOR_ENCHANT_ANNOUNCE;
	public static int MAX_WEAPON_ENCHANT_ANNOUNCE;

	public static ImmutableSortedSet<int> AUGMENTATION_BLACKLIST = ImmutableSortedSet<int>.Empty;
	public static bool ALT_ALLOW_AUGMENT_PVP_ITEMS;
	public static bool ALT_ALLOW_AUGMENT_TRADE;
	public static bool ALT_ALLOW_AUGMENT_DESTROY;
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
	public static byte[] HEX_ID;
	public static bool AUTO_CREATE_ACCOUNTS;
	public static bool FLOOD_PROTECTION;
	public static int FAST_CONNECTION_LIMIT;
	public static int NORMAL_CONNECTION_TIME;
	public static int FAST_CONNECTION_TIME;
	public static int MAX_CONNECTION_PER_IP;
	public static bool ENABLE_CMD_LINE_LOGIN;
	public static bool ONLY_CMD_LINE_LOGIN;
	public static bool RESURRECT_BY_PAYMENT_ENABLED;
	public static int RESURRECT_BY_PAYMENT_MAX_FREE_TIMES;
	public static int RESURRECT_BY_PAYMENT_FIRST_RESURRECT_ITEM;

	public static ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>
		RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES =
			ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>.Empty;

	public static int RESURRECT_BY_PAYMENT_SECOND_RESURRECT_ITEM;

	public static ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>
		RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES =
			ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>.Empty;

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
	public static string WORLD_EXCHANGE_DEFAULT_LANG;
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
	public static string GEODATA_PATH;
	public static string PATHNODE_PATH;
	public static int PATHFINDING;
	public static string PATHFIND_BUFFERS;
	public static float LOW_WEIGHT;
	public static float MEDIUM_WEIGHT;
	public static float HIGH_WEIGHT;
	public static bool ADVANCED_DIAGONAL_STRATEGY;
	public static float DIAGONAL_WEIGHT;
	public static int MAX_POSTFILTER_PASSES;
	public static bool DEBUG_PATH;

	/** Attribute System */
	public static int S_WEAPON_STONE;

	public static int S80_WEAPON_STONE;
	public static int S84_WEAPON_STONE;
	public static int R_WEAPON_STONE;
	public static int R95_WEAPON_STONE;
	public static int R99_WEAPON_STONE;

	public static int S_ARMOR_STONE;
	public static int S80_ARMOR_STONE;
	public static int S84_ARMOR_STONE;
	public static int R_ARMOR_STONE;
	public static int R95_ARMOR_STONE;
	public static int R99_ARMOR_STONE;

	public static int S_WEAPON_CRYSTAL;
	public static int S80_WEAPON_CRYSTAL;
	public static int S84_WEAPON_CRYSTAL;
	public static int R_WEAPON_CRYSTAL;
	public static int R95_WEAPON_CRYSTAL;
	public static int R99_WEAPON_CRYSTAL;

	public static int S_ARMOR_CRYSTAL;
	public static int S80_ARMOR_CRYSTAL;
	public static int S84_ARMOR_CRYSTAL;
	public static int R_ARMOR_CRYSTAL;
	public static int R95_ARMOR_CRYSTAL;
	public static int R99_ARMOR_CRYSTAL;

	public static int S_WEAPON_STONE_SUPER;
	public static int S80_WEAPON_STONE_SUPER;
	public static int S84_WEAPON_STONE_SUPER;
	public static int R_WEAPON_STONE_SUPER;
	public static int R95_WEAPON_STONE_SUPER;
	public static int R99_WEAPON_STONE_SUPER;

	public static int S_ARMOR_STONE_SUPER;
	public static int S80_ARMOR_STONE_SUPER;
	public static int S84_ARMOR_STONE_SUPER;
	public static int R_ARMOR_STONE_SUPER;
	public static int R95_ARMOR_STONE_SUPER;
	public static int R99_ARMOR_STONE_SUPER;

	public static int S_WEAPON_CRYSTAL_SUPER;
	public static int S80_WEAPON_CRYSTAL_SUPER;
	public static int S84_WEAPON_CRYSTAL_SUPER;
	public static int R_WEAPON_CRYSTAL_SUPER;
	public static int R95_WEAPON_CRYSTAL_SUPER;
	public static int R99_WEAPON_CRYSTAL_SUPER;

	public static int S_ARMOR_CRYSTAL_SUPER;
	public static int S80_ARMOR_CRYSTAL_SUPER;
	public static int S84_ARMOR_CRYSTAL_SUPER;
	public static int R_ARMOR_CRYSTAL_SUPER;
	public static int R95_ARMOR_CRYSTAL_SUPER;
	public static int R99_ARMOR_CRYSTAL_SUPER;

	public static int S_WEAPON_JEWEL;
	public static int S80_WEAPON_JEWEL;
	public static int S84_WEAPON_JEWEL;
	public static int R_WEAPON_JEWEL;
	public static int R95_WEAPON_JEWEL;
	public static int R99_WEAPON_JEWEL;

	public static int S_ARMOR_JEWEL;
	public static int S80_ARMOR_JEWEL;
	public static int S84_ARMOR_JEWEL;
	public static int R_ARMOR_JEWEL;
	public static int R95_ARMOR_JEWEL;
	public static int R99_ARMOR_JEWEL;

	// --------------------------------------------------
	// Custom Settings
	// --------------------------------------------------
	public static bool CHAMPION_ENABLE;
	public static bool CHAMPION_PASSIVE;
	public static int CHAMPION_FREQUENCY;
	public static string CHAMP_TITLE;
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
	public static string OFFLINE_PLAY_LOGIN_MESSAGE;
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
	public static string WELCOME_MESSAGE_TEXT;
	public static int WELCOME_MESSAGE_TIME;
	public static bool ANNOUNCE_PK_PVP;
	public static bool ANNOUNCE_PK_PVP_NORMAL_MESSAGE;
	public static string ANNOUNCE_PK_MSG;
	public static string ANNOUNCE_PVP_MSG;
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
	public static string TITLE_FOR_PVP_AMOUNT1;
	public static string TITLE_FOR_PVP_AMOUNT2;
	public static string TITLE_FOR_PVP_AMOUNT3;
	public static string TITLE_FOR_PVP_AMOUNT4;
	public static string TITLE_FOR_PVP_AMOUNT5;
	public static bool CHAT_ADMIN;

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
	public static string MULTILANG_DEFAULT;
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
	public static Map<int, int> DUALBOX_CHECK_WHITELIST;
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
	public static string FACTION_GOOD_TEAM_NAME;
	public static string FACTION_EVIL_TEAM_NAME;
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

	public static string SUBJUGATION_TOPIC_HEADER;
	public static string SUBJUGATION_TOPIC_BODY;

	public static bool ENABLE_GUI;
	public static bool DARK_THEME;

	/**
	 * This class initializes all global variables for configuration.<br>
	 * If the key doesn't appear in properties file, a default value is set by this class. {@link #SERVER_CONFIG_FILE} (properties file) for configuring your server.
	 * @param serverMode
	 */
	public static void Load(string? basePath = null)
	{
		Regex defaultRegex = new(".*", RegexOptions.Compiled | RegexOptions.NonBacktracking);

		ConfigurationParser parser = new(basePath);
		parser.LoadConfig(SERVER_CONFIG_FILE);

		GAMESERVER_HOSTNAME = parser.getString("GameserverHostname", "0.0.0.0");
		PORT_GAME = parser.getInt("GameserverPort", 7777);
		GAME_SERVER_LOGIN_PORT = parser.getInt("LoginPort", 9014);
		GAME_SERVER_LOGIN_HOST = parser.getString("LoginHost", "127.0.0.1");
		REQUEST_ID = parser.getInt("RequestServerID", 0);
		ACCEPT_ALTERNATE_ID = parser.getBoolean("AcceptAlternateID", true);
		DATABASE_DRIVER = parser.getString("Driver", "org.mariadb.jdbc.Driver");
		DATABASE_URL = parser.getString("URL", "jdbc:mariadb://localhost/l2jmobius");
		DATABASE_LOGIN = parser.getString("Login", "root");
		DATABASE_PASSWORD = parser.getString("Password", "");
		DATABASE_MAX_CONNECTIONS = parser.getInt("MaximumDbConnections", 10);
		BACKUP_DATABASE = parser.getBoolean("BackupDatabase", false);
		MYSQL_BIN_PATH = parser.getString("MySqlBinLocation", "C:/xampp/mysql/bin/");
		BACKUP_PATH = parser.getString("BackupPath", "../backup/");
		BACKUP_DAYS = parser.getInt("BackupDays", 30);
		DATAPACK_ROOT_PATH = parser.GetPath("DatapackRoot", ".");
		SCRIPT_ROOT_PATH = parser.GetPath("ScriptRoot", "./data/scripts");
		CHARNAME_TEMPLATE_PATTERN = parser.GetRegex("CnameTemplate", defaultRegex);
		PET_NAME_TEMPLATE = parser.GetRegex("PetNameTemplate", defaultRegex);
		CLAN_NAME_TEMPLATE = parser.GetRegex("ClanNameTemplate", defaultRegex);
		MAX_CHARACTERS_NUMBER_PER_ACCOUNT = parser.getInt("CharMaxNumber", 7);
		MAXIMUM_ONLINE_USERS = parser.getInt("MaximumOnlineUsers", 100);
		HARDWARE_INFO_ENABLED = parser.getBoolean("EnableHardwareInfo", false);
		KICK_MISSING_HWID = parser.getBoolean("KickMissingHWID", false);
		MAX_PLAYERS_PER_HWID = parser.getInt("MaxPlayersPerHWID", 0);
		if (MAX_PLAYERS_PER_HWID > 0)
		{
			KICK_MISSING_HWID = true;
		}

		PROTOCOL_LIST = parser.GetIntList("AllowedProtocolRevisions", ';', 447);
		SERVER_LIST_TYPE = parser.GetEnum("ServerListType", ServerType.Free);
		SERVER_LIST_AGE = parser.getInt("ServerListAge", 0);
		SERVER_LIST_BRACKET = parser.getBoolean("ServerListBrackets", false);
		SCHEDULED_THREAD_POOL_SIZE = parser.getInt("ScheduledThreadPoolSize", Environment.ProcessorCount * 4);
		INSTANT_THREAD_POOL_SIZE = parser.getInt("InstantThreadPoolSize", Environment.ProcessorCount * 2);
		THREADS_FOR_LOADING = parser.getBoolean("ThreadsForLoading", false);
		DEADLOCK_DETECTOR = parser.getBoolean("DeadLockDetector", true);
		DEADLOCK_CHECK_INTERVAL = parser.getInt("DeadLockCheckInterval", 20);
		RESTART_ON_DEADLOCK = parser.getBoolean("RestartOnDeadlock", false);
		SERVER_RESTART_SCHEDULE_ENABLED = parser.getBoolean("ServerRestartScheduleEnabled", false);
		SERVER_RESTART_SCHEDULE_MESSAGE = parser.getBoolean("ServerRestartScheduleMessage", false);
		SERVER_RESTART_SCHEDULE_COUNTDOWN = parser.getInt("ServerRestartScheduleCountdown", 600);
		SERVER_RESTART_SCHEDULE =
			parser.GetTimeList("ServerRestartSchedule", ',', TimeOnly.FromTimeSpan(TimeSpan.FromHours(8)));
		SERVER_RESTART_DAYS = parser.GetIntList("ServerRestartDays").Select(x => (DayOfWeek)((x - 1) % 7))
			.ToImmutableArray();

		PRECAUTIONARY_RESTART_ENABLED = parser.getBoolean("PrecautionaryRestartEnabled", false);
		PRECAUTIONARY_RESTART_CPU = parser.getBoolean("PrecautionaryRestartCpu", true);
		PRECAUTIONARY_RESTART_MEMORY = parser.getBoolean("PrecautionaryRestartMemory", false);
		PRECAUTIONARY_RESTART_CHECKS = parser.getBoolean("PrecautionaryRestartChecks", true);
		PRECAUTIONARY_RESTART_PERCENTAGE = parser.getInt("PrecautionaryRestartPercentage", 95);
		PRECAUTIONARY_RESTART_DELAY = parser.getInt("PrecautionaryRestartDelay", 60) * 1000;

		parser.LoadConfig(NETWORK_CONFIG_FILE);
		CLIENT_READ_POOL_SIZE = parser.getInt("ClientReadPoolSize", 100);
		CLIENT_SEND_POOL_SIZE = parser.getInt("ClientSendPoolSize", 100);
		CLIENT_EXECUTE_POOL_SIZE = parser.getInt("ClientExecutePoolSize", 100);
		PACKET_QUEUE_LIMIT = parser.getInt("PacketQueueLimit", 80);
		PACKET_FLOOD_DISCONNECT = parser.getBoolean("PacketFloodDisconnect", false);
		PACKET_FLOOD_DROP = parser.getBoolean("PacketFloodDrop", false);
		PACKET_FLOOD_LOGGED = parser.getBoolean("PacketFloodLogged", true);
		PACKET_ENCRYPTION = parser.getBoolean("PacketEncryption", false);
		FAILED_DECRYPTION_LOGGED = parser.getBoolean("FailedDecryptionLogged", true);
		TCP_NO_DELAY = parser.getBoolean("TcpNoDelay", true);

		// Hosts and Subnets
		// TODO: what we need here is the list of IP addresses to listen, and IP to report to the Login server

		// Load Feature config file (if exists)
		parser.LoadConfig(FEATURE_CONFIG_FILE);
		SIEGE_HOUR_LIST = parser.GetIntList("SiegeHourList");
		CASTLE_BUY_TAX_NEUTRAL = parser.getInt("BuyTaxForNeutralSide", 15);
		CASTLE_BUY_TAX_LIGHT = parser.getInt("BuyTaxForLightSide", 0);
		CASTLE_BUY_TAX_DARK = parser.getInt("BuyTaxForDarkSide", 30);
		CASTLE_SELL_TAX_NEUTRAL = parser.getInt("SellTaxForNeutralSide", 0);
		CASTLE_SELL_TAX_LIGHT = parser.getInt("SellTaxForLightSide", 0);
		CASTLE_SELL_TAX_DARK = parser.getInt("SellTaxForDarkSide", 20);
		CS_TELE_FEE_RATIO = parser.getLong("CastleTeleportFunctionFeeRatio", 604800000);
		CS_TELE1_FEE = parser.getInt("CastleTeleportFunctionFeeLvl1", 1000);
		CS_TELE2_FEE = parser.getInt("CastleTeleportFunctionFeeLvl2", 10000);
		CS_SUPPORT_FEE_RATIO = parser.getLong("CastleSupportFunctionFeeRatio", 604800000);
		CS_SUPPORT1_FEE = parser.getInt("CastleSupportFeeLvl1", 49000);
		CS_SUPPORT2_FEE = parser.getInt("CastleSupportFeeLvl2", 120000);
		CS_MPREG_FEE_RATIO = parser.getLong("CastleMpRegenerationFunctionFeeRatio", 604800000);
		CS_MPREG1_FEE = parser.getInt("CastleMpRegenerationFeeLvl1", 45000);
		CS_MPREG2_FEE = parser.getInt("CastleMpRegenerationFeeLvl2", 65000);
		CS_HPREG_FEE_RATIO = parser.getLong("CastleHpRegenerationFunctionFeeRatio", 604800000);
		CS_HPREG1_FEE = parser.getInt("CastleHpRegenerationFeeLvl1", 12000);
		CS_HPREG2_FEE = parser.getInt("CastleHpRegenerationFeeLvl2", 20000);
		CS_EXPREG_FEE_RATIO = parser.getLong("CastleExpRegenerationFunctionFeeRatio", 604800000);
		CS_EXPREG1_FEE = parser.getInt("CastleExpRegenerationFeeLvl1", 63000);
		CS_EXPREG2_FEE = parser.getInt("CastleExpRegenerationFeeLvl2", 70000);
		OUTER_DOOR_UPGRADE_PRICE2 = parser.getInt("OuterDoorUpgradePriceLvl2", 3000000);
		OUTER_DOOR_UPGRADE_PRICE3 = parser.getInt("OuterDoorUpgradePriceLvl3", 4000000);
		OUTER_DOOR_UPGRADE_PRICE5 = parser.getInt("OuterDoorUpgradePriceLvl5", 5000000);
		INNER_DOOR_UPGRADE_PRICE2 = parser.getInt("InnerDoorUpgradePriceLvl2", 750000);
		INNER_DOOR_UPGRADE_PRICE3 = parser.getInt("InnerDoorUpgradePriceLvl3", 900000);
		INNER_DOOR_UPGRADE_PRICE5 = parser.getInt("InnerDoorUpgradePriceLvl5", 1000000);
		WALL_UPGRADE_PRICE2 = parser.getInt("WallUpgradePriceLvl2", 1600000);
		WALL_UPGRADE_PRICE3 = parser.getInt("WallUpgradePriceLvl3", 1800000);
		WALL_UPGRADE_PRICE5 = parser.getInt("WallUpgradePriceLvl5", 2000000);
		TRAP_UPGRADE_PRICE1 = parser.getInt("TrapUpgradePriceLvl1", 3000000);
		TRAP_UPGRADE_PRICE2 = parser.getInt("TrapUpgradePriceLvl2", 4000000);
		TRAP_UPGRADE_PRICE3 = parser.getInt("TrapUpgradePriceLvl3", 5000000);
		TRAP_UPGRADE_PRICE4 = parser.getInt("TrapUpgradePriceLvl4", 6000000);
		FS_TELE_FEE_RATIO = parser.getLong("FortressTeleportFunctionFeeRatio", 604800000);
		FS_TELE1_FEE = parser.getInt("FortressTeleportFunctionFeeLvl1", 1000);
		FS_TELE2_FEE = parser.getInt("FortressTeleportFunctionFeeLvl2", 10000);
		FS_SUPPORT_FEE_RATIO = parser.getLong("FortressSupportFunctionFeeRatio", 86400000);
		FS_SUPPORT1_FEE = parser.getInt("FortressSupportFeeLvl1", 7000);
		FS_SUPPORT2_FEE = parser.getInt("FortressSupportFeeLvl2", 17000);
		FS_MPREG_FEE_RATIO = parser.getLong("FortressMpRegenerationFunctionFeeRatio", 86400000);
		FS_MPREG1_FEE = parser.getInt("FortressMpRegenerationFeeLvl1", 6500);
		FS_MPREG2_FEE = parser.getInt("FortressMpRegenerationFeeLvl2", 9300);
		FS_HPREG_FEE_RATIO = parser.getLong("FortressHpRegenerationFunctionFeeRatio", 86400000);
		FS_HPREG1_FEE = parser.getInt("FortressHpRegenerationFeeLvl1", 2000);
		FS_HPREG2_FEE = parser.getInt("FortressHpRegenerationFeeLvl2", 3500);
		FS_EXPREG_FEE_RATIO = parser.getLong("FortressExpRegenerationFunctionFeeRatio", 86400000);
		FS_EXPREG1_FEE = parser.getInt("FortressExpRegenerationFeeLvl1", 9000);
		FS_EXPREG2_FEE = parser.getInt("FortressExpRegenerationFeeLvl2", 10000);
		FS_UPDATE_FRQ = parser.getInt("FortressPeriodicUpdateFrequency", 360);
		FS_BLOOD_OATH_COUNT = parser.getInt("FortressBloodOathCount", 1);
		FS_MAX_SUPPLY_LEVEL = parser.getInt("FortressMaxSupplyLevel", 6);
		FS_FEE_FOR_CASTLE = parser.getInt("FortressFeeForCastle", 25000);
		FS_MAX_OWN_TIME = parser.getInt("FortressMaximumOwnTime", 168);
		TAKE_FORT_POINTS = parser.getInt("TakeFortPoints", 200);
		LOOSE_FORT_POINTS = parser.getInt("LooseFortPoints", 0);
		TAKE_CASTLE_POINTS = parser.getInt("TakeCastlePoints", 1500);
		LOOSE_CASTLE_POINTS = parser.getInt("LooseCastlePoints", 3000);
		CASTLE_DEFENDED_POINTS = parser.getInt("CastleDefendedPoints", 750);
		FESTIVAL_WIN_POINTS = parser.getInt("FestivalOfDarknessWin", 200);
		HERO_POINTS = parser.getInt("HeroPoints", 1000);
		ROYAL_GUARD_COST = parser.getInt("CreateRoyalGuardCost", 5000);
		KNIGHT_UNIT_COST = parser.getInt("CreateKnightUnitCost", 10000);
		KNIGHT_REINFORCE_COST = parser.getInt("ReinforceKnightUnitCost", 5000);
		BALLISTA_POINTS = parser.getInt("KillBallistaPoints", 500);
		BLOODALLIANCE_POINTS = parser.getInt("BloodAlliancePoints", 500);
		BLOODOATH_POINTS = parser.getInt("BloodOathPoints", 200);
		KNIGHTSEPAULETTE_POINTS = parser.getInt("KnightsEpaulettePoints", 20);
		REPUTATION_SCORE_PER_KILL = parser.getInt("ReputationScorePerKill", 1);
		JOIN_ACADEMY_MIN_REP_SCORE = parser.getInt("CompleteAcademyMinPoints", 190);
		JOIN_ACADEMY_MAX_REP_SCORE = parser.getInt("CompleteAcademyMaxPoints", 650);
		LVL_UP_20_AND_25_REP_SCORE = parser.getInt("LevelUp20And25ReputationScore", 4);
		LVL_UP_26_AND_30_REP_SCORE = parser.getInt("LevelUp26And30ReputationScore", 8);
		LVL_UP_31_AND_35_REP_SCORE = parser.getInt("LevelUp31And35ReputationScore", 12);
		LVL_UP_36_AND_40_REP_SCORE = parser.getInt("LevelUp36And40ReputationScore", 16);
		LVL_UP_41_AND_45_REP_SCORE = parser.getInt("LevelUp41And45ReputationScore", 25);
		LVL_UP_46_AND_50_REP_SCORE = parser.getInt("LevelUp46And50ReputationScore", 30);
		LVL_UP_51_AND_55_REP_SCORE = parser.getInt("LevelUp51And55ReputationScore", 35);
		LVL_UP_56_AND_60_REP_SCORE = parser.getInt("LevelUp56And60ReputationScore", 40);
		LVL_UP_61_AND_65_REP_SCORE = parser.getInt("LevelUp61And65ReputationScore", 54);
		LVL_UP_66_AND_70_REP_SCORE = parser.getInt("LevelUp66And70ReputationScore", 63);
		LVL_UP_71_AND_75_REP_SCORE = parser.getInt("LevelUp71And75ReputationScore", 75);
		LVL_UP_76_AND_80_REP_SCORE = parser.getInt("LevelUp76And80ReputationScore", 90);
		LVL_UP_81_AND_90_REP_SCORE = parser.getInt("LevelUp81And90ReputationScore", 120);
		LVL_UP_91_PLUS_REP_SCORE = parser.getInt("LevelUp91PlusReputationScore", 150);
		LVL_OBTAINED_REP_SCORE_MULTIPLIER = parser.getDouble("LevelObtainedReputationScoreMultiplier", 1.0);
		CLAN_LEVEL_6_COST = parser.getInt("ClanLevel6Cost", 15000);
		CLAN_LEVEL_7_COST = parser.getInt("ClanLevel7Cost", 450000);
		CLAN_LEVEL_8_COST = parser.getInt("ClanLevel8Cost", 1000000);
		CLAN_LEVEL_9_COST = parser.getInt("ClanLevel9Cost", 2000000);
		CLAN_LEVEL_10_COST = parser.getInt("ClanLevel10Cost", 4000000);
		CLAN_LEVEL_6_REQUIREMENT = parser.getInt("ClanLevel6Requirement", 40);
		CLAN_LEVEL_7_REQUIREMENT = parser.getInt("ClanLevel7Requirement", 40);
		CLAN_LEVEL_8_REQUIREMENT = parser.getInt("ClanLevel8Requirement", 40);
		CLAN_LEVEL_9_REQUIREMENT = parser.getInt("ClanLevel9Requirement", 40);
		CLAN_LEVEL_10_REQUIREMENT = parser.getInt("ClanLevel10Requirement", 40);
		ALLOW_WYVERN_ALWAYS = parser.getBoolean("AllowRideWyvernAlways", false);
		ALLOW_WYVERN_DURING_SIEGE = parser.getBoolean("AllowRideWyvernDuringSiege", true);
		ALLOW_MOUNTS_DURING_SIEGE = parser.getBoolean("AllowRideMountsDuringSiege", false);

		// Load Attendance config file (if exists)
		parser.LoadConfig(ATTENDANCE_CONFIG_FILE);
		ENABLE_ATTENDANCE_REWARDS = parser.getBoolean("EnableAttendanceRewards", false);
		PREMIUM_ONLY_ATTENDANCE_REWARDS = parser.getBoolean("PremiumOnlyAttendanceRewards", false);
		VIP_ONLY_ATTENDANCE_REWARDS = parser.getBoolean("VipOnlyAttendanceRewards", false);
		ATTENDANCE_REWARDS_SHARE_ACCOUNT = parser.getBoolean("AttendanceRewardsShareAccount", false);
		ATTENDANCE_REWARD_DELAY = parser.getInt("AttendanceRewardDelay", 30);
		ATTENDANCE_POPUP_START = parser.getBoolean("AttendancePopupStart", true);
		ATTENDANCE_POPUP_WINDOW = parser.getBoolean("AttendancePopupWindow", false);

		// Load AttributeSystem config file (if exists)
		parser.LoadConfig(ATTRIBUTE_SYSTEM_FILE);
		S_WEAPON_STONE = parser.getInt("SWeaponStone", 50);
		S80_WEAPON_STONE = parser.getInt("S80WeaponStone", 50);
		S84_WEAPON_STONE = parser.getInt("S84WeaponStone", 50);
		R_WEAPON_STONE = parser.getInt("RWeaponStone", 50);
		R95_WEAPON_STONE = parser.getInt("R95WeaponStone", 50);
		R99_WEAPON_STONE = parser.getInt("R99WeaponStone", 50);
		S_ARMOR_STONE = parser.getInt("SArmorStone", 60);
		S80_ARMOR_STONE = parser.getInt("S80ArmorStone", 80);
		S84_ARMOR_STONE = parser.getInt("S84ArmorStone", 80);
		R_ARMOR_STONE = parser.getInt("RArmorStone", 100);
		R95_ARMOR_STONE = parser.getInt("R95ArmorStone", 100);
		R99_ARMOR_STONE = parser.getInt("R99ArmorStone", 100);
		S_WEAPON_CRYSTAL = parser.getInt("SWeaponCrystal", 30);
		S80_WEAPON_CRYSTAL = parser.getInt("S80WeaponCrystal", 40);
		S84_WEAPON_CRYSTAL = parser.getInt("S84WeaponCrystal", 50);
		R_WEAPON_CRYSTAL = parser.getInt("RWeaponCrystal", 60);
		R95_WEAPON_CRYSTAL = parser.getInt("R95WeaponCrystal", 60);
		R99_WEAPON_CRYSTAL = parser.getInt("R99WeaponCrystal", 60);
		S_ARMOR_CRYSTAL = parser.getInt("SArmorCrystal", 50);
		S80_ARMOR_CRYSTAL = parser.getInt("S80ArmorCrystal", 70);
		S84_ARMOR_CRYSTAL = parser.getInt("S84ArmorCrystal", 80);
		R_ARMOR_CRYSTAL = parser.getInt("RArmorCrystal", 80);
		R95_ARMOR_CRYSTAL = parser.getInt("R95ArmorCrystal", 100);
		R99_ARMOR_CRYSTAL = parser.getInt("R99ArmorCrystal", 100);
		S_WEAPON_STONE_SUPER = parser.getInt("SWeaponStoneSuper", 100);
		S80_WEAPON_STONE_SUPER = parser.getInt("S80WeaponStoneSuper", 100);
		S84_WEAPON_STONE_SUPER = parser.getInt("S84WeaponStoneSuper", 100);
		R_WEAPON_STONE_SUPER = parser.getInt("RWeaponStoneSuper", 100);
		R95_WEAPON_STONE_SUPER = parser.getInt("R95WeaponStoneSuper", 100);
		R99_WEAPON_STONE_SUPER = parser.getInt("R99WeaponStoneSuper", 100);
		S_ARMOR_STONE_SUPER = parser.getInt("SArmorStoneSuper", 100);
		S80_ARMOR_STONE_SUPER = parser.getInt("S80ArmorStoneSuper", 100);
		S84_ARMOR_STONE_SUPER = parser.getInt("S84ArmorStoneSuper", 100);
		R_ARMOR_STONE_SUPER = parser.getInt("RArmorStoneSuper", 100);
		R95_ARMOR_STONE_SUPER = parser.getInt("R95ArmorStoneSuper", 100);
		R99_ARMOR_STONE_SUPER = parser.getInt("R99ArmorStoneSuper", 100);
		S_WEAPON_CRYSTAL_SUPER = parser.getInt("SWeaponCrystalSuper", 80);
		S80_WEAPON_CRYSTAL_SUPER = parser.getInt("S80WeaponCrystalSuper", 90);
		S84_WEAPON_CRYSTAL_SUPER = parser.getInt("S84WeaponCrystalSuper", 100);
		R_WEAPON_CRYSTAL_SUPER = parser.getInt("RWeaponCrystalSuper", 100);
		R95_WEAPON_CRYSTAL_SUPER = parser.getInt("R95WeaponCrystalSuper", 100);
		R99_WEAPON_CRYSTAL_SUPER = parser.getInt("R99WeaponCrystalSuper", 100);
		S_ARMOR_CRYSTAL_SUPER = parser.getInt("SArmorCrystalSuper", 100);
		S80_ARMOR_CRYSTAL_SUPER = parser.getInt("S80ArmorCrystalSuper", 100);
		S84_ARMOR_CRYSTAL_SUPER = parser.getInt("S84ArmorCrystalSuper", 100);
		R_ARMOR_CRYSTAL_SUPER = parser.getInt("RArmorCrystalSuper", 100);
		R95_ARMOR_CRYSTAL_SUPER = parser.getInt("R95ArmorCrystalSuper", 100);
		R99_ARMOR_CRYSTAL_SUPER = parser.getInt("R99ArmorCrystalSuper", 100);
		S_WEAPON_JEWEL = parser.getInt("SWeaponJewel", 100);
		S80_WEAPON_JEWEL = parser.getInt("S80WeaponJewel", 100);
		S84_WEAPON_JEWEL = parser.getInt("S84WeaponJewel", 100);
		R_WEAPON_JEWEL = parser.getInt("RWeaponJewel", 100);
		R95_WEAPON_JEWEL = parser.getInt("R95WeaponJewel", 100);
		R99_WEAPON_JEWEL = parser.getInt("R99WeaponJewel", 100);
		S_ARMOR_JEWEL = parser.getInt("SArmorJewel", 100);
		S80_ARMOR_JEWEL = parser.getInt("S80ArmorJewel", 100);
		S84_ARMOR_JEWEL = parser.getInt("S84ArmorJewel", 100);
		R_ARMOR_JEWEL = parser.getInt("RArmorJewel", 100);
		R95_ARMOR_JEWEL = parser.getInt("R95ArmorJewel", 100);
		R99_ARMOR_JEWEL = parser.getInt("R99ArmorJewel", 100);

		// Load Character config file (if exists)
		parser.LoadConfig(CHARACTER_CONFIG_FILE);
		PLAYER_DELEVEL = parser.getBoolean("Delevel", true);
		DELEVEL_MINIMUM = parser.getInt("DelevelMinimum", 85);
		DECREASE_SKILL_LEVEL = parser.getBoolean("DecreaseSkillOnDelevel", true);
		ALT_WEIGHT_LIMIT = parser.getDouble("AltWeightLimit", 1);
		RUN_SPD_BOOST = parser.getInt("RunSpeedBoost", 0);
		RESPAWN_RESTORE_CP = parser.getDouble("RespawnRestoreCP", 0) / 100;
		RESPAWN_RESTORE_HP = parser.getDouble("RespawnRestoreHP", 65) / 100;
		RESPAWN_RESTORE_MP = parser.getDouble("RespawnRestoreMP", 0) / 100;
		HP_REGEN_MULTIPLIER = parser.getDouble("HpRegenMultiplier", 100) / 100;
		MP_REGEN_MULTIPLIER = parser.getDouble("MpRegenMultiplier", 100) / 100;
		CP_REGEN_MULTIPLIER = parser.getDouble("CpRegenMultiplier", 100) / 100;

		RESURRECT_BY_PAYMENT_ENABLED = parser.getBoolean("EnabledResurrectByPay", true);
		if (RESURRECT_BY_PAYMENT_ENABLED)
		{
			RESURRECT_BY_PAYMENT_MAX_FREE_TIMES = parser.getInt("MaxFreeResurrectionsByDay", 0);
			RESURRECT_BY_PAYMENT_FIRST_RESURRECT_ITEM = parser.getInt("FirstResurrectItemID", 0);
			RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES = GetResurrectByPaymentList(parser, "FirstResurrectList");
			RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES = GetResurrectByPaymentList(parser, "SecondResurrectList");
			RESURRECT_BY_PAYMENT_SECOND_RESURRECT_ITEM = parser.getInt("SecondResurrectItemID", 0);
		}

		ENABLE_MODIFY_SKILL_DURATION = parser.getBoolean("EnableModifySkillDuration", false);
		if (ENABLE_MODIFY_SKILL_DURATION)
		{
			SKILL_DURATION_LIST = GetSkillDurationList(parser, "SkillDurationList");
		}

		ENABLE_MODIFY_SKILL_REUSE = parser.getBoolean("EnableModifySkillReuse", false);
		if (ENABLE_MODIFY_SKILL_REUSE)
		{
			SKILL_REUSE_LIST = GetSkillDurationList(parser, "SkillReuseList");
		}

		AUTO_LEARN_SKILLS = parser.getBoolean("AutoLearnSkills", false);
		AUTO_LEARN_SKILLS_WITHOUT_ITEMS = parser.getBoolean("AutoLearnSkillsWithoutItems", false);
		AUTO_LEARN_FS_SKILLS = parser.getBoolean("AutoLearnForgottenScrollSkills", false);
		AUTO_LOOT_HERBS = parser.getBoolean("AutoLootHerbs", false);
		BUFFS_MAX_AMOUNT = parser.getByte("MaxBuffAmount", 20);
		TRIGGERED_BUFFS_MAX_AMOUNT = parser.getByte("MaxTriggeredBuffAmount", 12);
		DANCES_MAX_AMOUNT = parser.getByte("MaxDanceAmount", 12);
		DANCE_CANCEL_BUFF = parser.getBoolean("DanceCancelBuff", false);
		DANCE_CONSUME_ADDITIONAL_MP = parser.getBoolean("DanceConsumeAdditionalMP", true);
		ALT_STORE_DANCES = parser.getBoolean("AltStoreDances", false);
		AUTO_LEARN_DIVINE_INSPIRATION = parser.getBoolean("AutoLearnDivineInspiration", false);
		ALT_GAME_CANCEL_BOW = parser.getString("AltGameCancelByHit", "Cast").equalsIgnoreCase("bow") ||
		                      parser.getString("AltGameCancelByHit", "Cast").equalsIgnoreCase("all");
		ALT_GAME_CANCEL_CAST = parser.getString("AltGameCancelByHit", "Cast").equalsIgnoreCase("cast") ||
		                       parser.getString("AltGameCancelByHit", "Cast").equalsIgnoreCase("all");
		ALT_GAME_MAGICFAILURES = parser.getBoolean("MagicFailures", true);
		ALT_GAME_STUN_BREAK = parser.getBoolean("BreakStun", false);
		PLAYER_FAKEDEATH_UP_PROTECTION = parser.getInt("PlayerFakeDeathUpProtection", 0);
		STORE_SKILL_COOLTIME = parser.getBoolean("StoreSkillCooltime", true);
		SUBCLASS_STORE_SKILL_COOLTIME = parser.getBoolean("SubclassStoreSkillCooltime", false);
		SUMMON_STORE_SKILL_COOLTIME = parser.getBoolean("SummonStoreSkillCooltime", true);
		EFFECT_TICK_RATIO = parser.getLong("EffectTickRatio", 666);
		FAKE_DEATH_UNTARGET = parser.getBoolean("FakeDeathUntarget", true);
		FAKE_DEATH_DAMAGE_STAND = parser.getBoolean("FakeDeathDamageStand", false);
		VAMPIRIC_ATTACK_WORKS_WITH_SKILLS = parser.getBoolean("VampiricAttackWorkWithSkills", true);
		MP_VAMPIRIC_ATTACK_WORKS_WITH_MELEE = parser.getBoolean("MpVampiricAttackWorkWithMelee", false);
		CALCULATE_MAGIC_SUCCESS_BY_SKILL_MAGIC_LEVEL =
			parser.getBoolean("CalculateMagicSuccessBySkillMagicLevel", false);
		BLOW_RATE_CHANCE_LIMIT = parser.getInt("BlowRateChanceLimit", 100);
		ITEM_EQUIP_ACTIVE_SKILL_REUSE = parser.getInt("ItemEquipActiveSkillReuse", 300000);
		ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE = parser.getInt("ArmorSetEquipActiveSkillReuse", 60000);
		PLAYER_REFLECT_PERCENT_LIMIT = parser.getDouble("PlayerReflectPercentLimit", 100);
		NON_PLAYER_REFLECT_PERCENT_LIMIT = parser.getDouble("NonPlayerReflectPercentLimit", 100);
		LIFE_CRYSTAL_NEEDED = parser.getBoolean("LifeCrystalNeeded", true);
		DIVINE_SP_BOOK_NEEDED = parser.getBoolean("DivineInspirationSpBookNeeded", true);
		ALT_GAME_SUBCLASS_WITHOUT_QUESTS = parser.getBoolean("AltSubClassWithoutQuests", false);
		ALT_GAME_SUBCLASS_EVERYWHERE = parser.getBoolean("AltSubclassEverywhere", false);
		RESTORE_SERVITOR_ON_RECONNECT = parser.getBoolean("RestoreServitorOnReconnect", true);
		RESTORE_PET_ON_RECONNECT = parser.getBoolean("RestorePetOnReconnect", true);
		ALLOW_TRANSFORM_WITHOUT_QUEST = parser.getBoolean("AltTransformationWithoutQuest", false);
		FEE_DELETE_TRANSFER_SKILLS = parser.getInt("FeeDeleteTransferSkills", 10000000);
		FEE_DELETE_SUBCLASS_SKILLS = parser.getInt("FeeDeleteSubClassSkills", 10000000);
		FEE_DELETE_DUALCLASS_SKILLS = parser.getInt("FeeDeleteDualClassSkills", 20000000);
		ENABLE_VITALITY = parser.getBoolean("EnableVitality", true);
		STARTING_VITALITY_POINTS = parser.getInt("StartingVitalityPoints", 140000);
		RAIDBOSS_USE_VITALITY = parser.getBoolean("RaidbossUseVitality", true);
		MAX_BONUS_EXP = parser.getDouble("MaxExpBonus", 0);
		MAX_BONUS_SP = parser.getDouble("MaxSpBonus", 0);
		MAX_RUN_SPEED = parser.getInt("MaxRunSpeed", 300);
		MAX_RUN_SPEED_SUMMON = parser.getInt("MaxRunSpeedSummon", 350);
		MAX_PATK = parser.getInt("MaxPAtk", 999999);
		MAX_MATK = parser.getInt("MaxMAtk", 999999);
		MAX_PCRIT_RATE = parser.getInt("MaxPCritRate", 500);
		MAX_MCRIT_RATE = parser.getInt("MaxMCritRate", 200);
		MAX_PATK_SPEED = parser.getInt("MaxPAtkSpeed", 1500);
		MAX_MATK_SPEED = parser.getInt("MaxMAtkSpeed", 1999);
		MAX_EVASION = parser.getInt("MaxEvasion", 250);
		MAX_HP = parser.getInt("MaxHP", 150000);
		MIN_ABNORMAL_STATE_SUCCESS_RATE = parser.getInt("MinAbnormalStateSuccessRate", 10);
		MAX_ABNORMAL_STATE_SUCCESS_RATE = parser.getInt("MaxAbnormalStateSuccessRate", 90);
		MAX_SP = parser.getLong("MaxSp", 50000000000L) >= 0 ? parser.getLong("MaxSp", 50000000000L) : long.MaxValue;
		PLAYER_MAXIMUM_LEVEL = parser.getInt("MaximumPlayerLevel", 99);
		PLAYER_MAXIMUM_LEVEL++;
		MAX_SUBCLASS = Math.Min(3, parser.getInt("MaxSubclass", 3));
		BASE_SUBCLASS_LEVEL = parser.getInt("BaseSubclassLevel", 40);
		BASE_DUALCLASS_LEVEL = parser.getInt("BaseDualclassLevel", 85);
		MAX_SUBCLASS_LEVEL = parser.getInt("MaxSubclassLevel", 80);
		MAX_PVTSTORESELL_SLOTS_DWARF = parser.getInt("MaxPvtStoreSellSlotsDwarf", 4);
		MAX_PVTSTORESELL_SLOTS_OTHER = parser.getInt("MaxPvtStoreSellSlotsOther", 3);
		MAX_PVTSTOREBUY_SLOTS_DWARF = parser.getInt("MaxPvtStoreBuySlotsDwarf", 5);
		MAX_PVTSTOREBUY_SLOTS_OTHER = parser.getInt("MaxPvtStoreBuySlotsOther", 4);
		INVENTORY_MAXIMUM_NO_DWARF = parser.getInt("MaximumSlotsForNoDwarf", 80);
		INVENTORY_MAXIMUM_DWARF = parser.getInt("MaximumSlotsForDwarf", 100);
		INVENTORY_MAXIMUM_GM = parser.getInt("MaximumSlotsForGMPlayer", 250);
		INVENTORY_MAXIMUM_QUEST_ITEMS = parser.getInt("MaximumSlotsForQuestItems", 100);
		MAX_ITEM_IN_PACKET =
			Math.Max(INVENTORY_MAXIMUM_NO_DWARF, Math.Max(INVENTORY_MAXIMUM_DWARF, INVENTORY_MAXIMUM_GM));
		WAREHOUSE_SLOTS_DWARF = parser.getInt("MaximumWarehouseSlotsForDwarf", 120);
		WAREHOUSE_SLOTS_NO_DWARF = parser.getInt("MaximumWarehouseSlotsForNoDwarf", 100);
		WAREHOUSE_SLOTS_CLAN = parser.getInt("MaximumWarehouseSlotsForClan", 150);
		ALT_FREIGHT_SLOTS = parser.getInt("MaximumFreightSlots", 200);
		ALT_FREIGHT_PRICE = parser.getInt("FreightPrice", 1000);
		MENTOR_PENALTY_FOR_MENTEE_COMPLETE = parser.getInt("MentorPenaltyForMenteeComplete", 1) * 24 * 60 * 60 * 1000;
		MENTOR_PENALTY_FOR_MENTEE_COMPLETE = parser.getInt("MentorPenaltyForMenteeLeave", 2) * 24 * 60 * 60 * 1000;
		ENCHANT_BLACKLIST = parser.GetIntList("EnchantBlackList", ',', 7816, 7817, 7818, 7819, 7820, 7821, 7822, 7823,
			7824, 7825, 7826, 7827, 7828, 7829, 7830, 7831, 13293, 13294, 13296).ToImmutableSortedSet();

		DISABLE_OVER_ENCHANTING = parser.getBoolean("DisableOverEnchanting", true);
		OVER_ENCHANT_PROTECTION = parser.getBoolean("OverEnchantProtection", true);
		OVER_ENCHANT_PUNISHMENT = parser.GetEnum("OverEnchantPunishment", IllegalActionPunishmentType.JAIL);
		MIN_ARMOR_ENCHANT_ANNOUNCE = parser.getInt("MinimumArmorEnchantAnnounce", 6);
		MIN_WEAPON_ENCHANT_ANNOUNCE = parser.getInt("MinimumWeaponEnchantAnnounce", 7);
		MAX_ARMOR_ENCHANT_ANNOUNCE = parser.getInt("MaximumArmorEnchantAnnounce", 30);
		MAX_WEAPON_ENCHANT_ANNOUNCE = parser.getInt("MaximumWeaponEnchantAnnounce", 30);
		AUGMENTATION_BLACKLIST = parser.GetIntList("AugmentationBlackList").ToImmutableSortedSet();
		ALT_ALLOW_AUGMENT_PVP_ITEMS = parser.getBoolean("AltAllowAugmentPvPItems", false);
		ALT_ALLOW_AUGMENT_TRADE = parser.getBoolean("AltAllowAugmentTrade", false);
		ALT_ALLOW_AUGMENT_DESTROY = parser.getBoolean("AltAllowAugmentDestroy", true);
		ALT_GAME_KARMA_PLAYER_CAN_BE_KILLED_IN_PEACEZONE =
			parser.getBoolean("AltKarmaPlayerCanBeKilledInPeaceZone", false);
		ALT_GAME_KARMA_PLAYER_CAN_SHOP = parser.getBoolean("AltKarmaPlayerCanShop", true);
		ALT_GAME_KARMA_PLAYER_CAN_TELEPORT = parser.getBoolean("AltKarmaPlayerCanTeleport", true);
		ALT_GAME_KARMA_PLAYER_CAN_USE_GK = parser.getBoolean("AltKarmaPlayerCanUseGK", false);
		ALT_GAME_KARMA_PLAYER_CAN_TRADE = parser.getBoolean("AltKarmaPlayerCanTrade", true);
		ALT_GAME_KARMA_PLAYER_CAN_USE_WAREHOUSE = parser.getBoolean("AltKarmaPlayerCanUseWareHouse", true);
		MAX_PERSONAL_FAME_POINTS = parser.getInt("MaxPersonalFamePoints", 100000);
		FORTRESS_ZONE_FAME_TASK_FREQUENCY = parser.getInt("FortressZoneFameTaskFrequency", 300);
		FORTRESS_ZONE_FAME_AQUIRE_POINTS = parser.getInt("FortressZoneFameAquirePoints", 31);
		CASTLE_ZONE_FAME_TASK_FREQUENCY = parser.getInt("CastleZoneFameTaskFrequency", 300);
		CASTLE_ZONE_FAME_AQUIRE_POINTS = parser.getInt("CastleZoneFameAquirePoints", 125);
		FAME_FOR_DEAD_PLAYERS = parser.getBoolean("FameForDeadPlayers", true);
		IS_CRAFTING_ENABLED = parser.getBoolean("CraftingEnabled", true);
		CRAFT_MASTERWORK = parser.getBoolean("CraftMasterwork", true);
		DWARF_RECIPE_LIMIT = parser.getInt("DwarfRecipeLimit", 50);
		COMMON_RECIPE_LIMIT = parser.getInt("CommonRecipeLimit", 50);
		ALT_GAME_CREATION = parser.getBoolean("AltGameCreation", false);
		ALT_GAME_CREATION_SPEED = parser.getDouble("AltGameCreationSpeed", 1);
		ALT_GAME_CREATION_XP_RATE = parser.getDouble("AltGameCreationXpRate", 1);
		ALT_GAME_CREATION_SP_RATE = parser.getDouble("AltGameCreationSpRate", 1);
		ALT_GAME_CREATION_RARE_XPSP_RATE = parser.getDouble("AltGameCreationRareXpSpRate", 2);
		ALT_CLAN_LEADER_INSTANT_ACTIVATION = parser.getBoolean("AltClanLeaderInstantActivation", false);
		ALT_CLAN_JOIN_MINS = parser.getInt("MinutesBeforeJoinAClan", 1);
		ALT_CLAN_CREATE_DAYS = parser.getInt("DaysBeforeCreateAClan", 10);
		ALT_CLAN_DISSOLVE_DAYS = parser.getInt("DaysToPassToDissolveAClan", 7);
		ALT_ALLY_JOIN_DAYS_WHEN_LEAVED = parser.getInt("DaysBeforeJoinAllyWhenLeaved", 1);
		ALT_ALLY_JOIN_DAYS_WHEN_DISMISSED = parser.getInt("DaysBeforeJoinAllyWhenDismissed", 1);
		ALT_ACCEPT_CLAN_DAYS_WHEN_DISMISSED = parser.getInt("DaysBeforeAcceptNewClanWhenDismissed", 1);
		ALT_CREATE_ALLY_DAYS_WHEN_DISSOLVED = parser.getInt("DaysBeforeCreateNewAllyWhenDissolved", 1);
		ALT_MAX_NUM_OF_CLANS_IN_ALLY = parser.getInt("AltMaxNumOfClansInAlly", 3);
		ALT_CLAN_MEMBERS_FOR_WAR = parser.getInt("AltClanMembersForWar", 15);
		ALT_MEMBERS_CAN_WITHDRAW_FROM_CLANWH = parser.getBoolean("AltMembersCanWithdrawFromClanWH", false);
		ALT_CLAN_MEMBERS_TIME_FOR_BONUS = parser.GetTimeSpan("AltClanMembersTimeForBonus", TimeSpan.FromMinutes(30));
		REMOVE_CASTLE_CIRCLETS = parser.getBoolean("RemoveCastleCirclets", true);
		ALT_PARTY_MAX_MEMBERS = parser.getInt("AltPartyMaxMembers", 7);
		ALT_PARTY_RANGE = parser.getInt("AltPartyRange", 1500);
		ALT_LEAVE_PARTY_LEADER = parser.getBoolean("AltLeavePartyLeader", false);
		ALT_COMMAND_CHANNEL_FRIENDS = parser.getBoolean("AltCommandChannelFriends", false);
		INITIAL_EQUIPMENT_EVENT = parser.getBoolean("InitialEquipmentEvent", false);
		STARTING_ADENA = parser.getLong("StartingAdena", 0);
		STARTING_LEVEL = parser.getInt("StartingLevel", 1);
		STARTING_SP = parser.getInt("StartingSP", 0);
		MAX_ADENA = parser.getLong("MaxAdena", 99900000000L);
		if (MAX_ADENA < 0)
		{
			MAX_ADENA = long.MaxValue;
		}

		AUTO_LOOT = parser.getBoolean("AutoLoot", false);
		AUTO_LOOT_RAIDS = parser.getBoolean("AutoLootRaids", false);
		AUTO_LOOT_SLOT_LIMIT = parser.getBoolean("AutoLootSlotLimit", false);
		LOOT_RAIDS_PRIVILEGE_INTERVAL = parser.getInt("RaidLootRightsInterval", 900) * 1000;
		LOOT_RAIDS_PRIVILEGE_CC_SIZE = parser.getInt("RaidLootRightsCCSize", 45);
		AUTO_LOOT_ITEM_IDS = parser.GetIntList("AutoLootItemIds").ToImmutableSortedSet();
		ENABLE_KEYBOARD_MOVEMENT = parser.getBoolean("KeyboardMovement", true);
		UNSTUCK_INTERVAL = parser.getInt("UnstuckInterval", 300);
		TELEPORT_WATCHDOG_TIMEOUT = parser.getInt("TeleportWatchdogTimeout", 0);
		PLAYER_SPAWN_PROTECTION = parser.getInt("PlayerSpawnProtection", 0);
		PLAYER_TELEPORT_PROTECTION = parser.getInt("PlayerTeleportProtection", 0);
		RANDOM_RESPAWN_IN_TOWN_ENABLED = parser.getBoolean("RandomRespawnInTownEnabled", true);
		OFFSET_ON_TELEPORT_ENABLED = parser.getBoolean("OffsetOnTeleportEnabled", true);
		MAX_OFFSET_ON_TELEPORT = parser.getInt("MaxOffsetOnTeleport", 50);
		TELEPORT_WHILE_SIEGE_IN_PROGRESS = parser.getBoolean("TeleportWhileSiegeInProgress", true);
		TELEPORT_WHILE_PLAYER_IN_COMBAT = parser.getBoolean("TeleportWhilePlayerInCombat", false);
		PETITIONING_ALLOWED = parser.getBoolean("PetitioningAllowed", true);
		MAX_PETITIONS_PER_PLAYER = parser.getInt("MaxPetitionsPerPlayer", 5);
		MAX_PETITIONS_PENDING = parser.getInt("MaxPetitionsPending", 25);
		MAX_FREE_TELEPORT_LEVEL = parser.getInt("MaxFreeTeleportLevel", 99);
		MAX_NEWBIE_BUFF_LEVEL = parser.getInt("MaxNewbieBuffLevel", 0);
		DELETE_DAYS = parser.getInt("DeleteCharAfterDays", 1);
		DISCONNECT_AFTER_DEATH = parser.getBoolean("DisconnectAfterDeath", true);
		PARTY_XP_CUTOFF_METHOD = parser.getString("PartyXpCutoffMethod", "level").ToLower();
		PARTY_XP_CUTOFF_PERCENT = parser.getDouble("PartyXpCutoffPercent", 3);
		PARTY_XP_CUTOFF_LEVEL = parser.getInt("PartyXpCutoffLevel", 20);
		PARTY_XP_CUTOFF_GAPS = GetPartyXpCutoffGaps(parser, "PartyXpCutoffGaps",
			new(0, 9), new(10, 14), new(15, 99));

		PARTY_XP_CUTOFF_GAP_PERCENTS = parser.GetIntList("PartyXpCutoffGapPercent", ';', 100, 30, 0);
		DISABLE_TUTORIAL = parser.getBoolean("DisableTutorial", false);
		STORE_RECIPE_SHOPLIST = parser.getBoolean("StoreRecipeShopList", false);
		STORE_UI_SETTINGS = parser.getBoolean("StoreCharUiSettings", true);
		FORBIDDEN_NAMES = parser.GetStringList("ForbiddenNames")
			.ToImmutableSortedSet(StringComparer.CurrentCultureIgnoreCase);
		SILENCE_MODE_EXCLUDE = parser.getBoolean("SilenceModeExclude", false);
		PLAYER_MOVEMENT_BLOCK_TIME = parser.getInt("NpcTalkBlockingTime", 0) * 1000;
		ABILITY_MAX_POINTS = parser.getInt("AbilityMaxPoints", 16);
		ABILITY_POINTS_RESET_ADENA = parser.getLong("AbilityPointsResetAdena", 10_000_000);

		// Load Magic Lamp config file (if exists)
		parser.LoadConfig(MAGIC_LAMP_FILE);
		ENABLE_MAGIC_LAMP = parser.getBoolean("MagicLampEnabled", false);
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
		WORLD_EXCHANGE_LAZY_UPDATE = parser.getBoolean("DBLazy", false);
		WORLD_EXCHANGE_ITEM_SELL_PERIOD = parser.getInt("ItemSellPeriod", 14);
		WORLD_EXCHANGE_ITEM_BACK_PERIOD = parser.getInt("ItemBackPeriod", 120);
		WORLD_EXCHANGE_PAYMENT_TAKE_PERIOD = parser.getInt("PaymentTakePeriod", 120);

		// Load Training Camp config file (if exists)
		parser.LoadConfig(TRAINING_CAMP_CONFIG_FILE);
		TRAINING_CAMP_ENABLE = parser.getBoolean("TrainingCampEnable", false);
		TRAINING_CAMP_PREMIUM_ONLY = parser.getBoolean("TrainingCampPremiumOnly", false);
		TRAINING_CAMP_MAX_DURATION = parser.getInt("TrainingCampDuration", 18000);
		TRAINING_CAMP_MIN_LEVEL = parser.getInt("TrainingCampMinLevel", 18);
		TRAINING_CAMP_MAX_LEVEL = parser.getInt("TrainingCampMaxLevel", 127);
		TRAINING_CAMP_EXP_MULTIPLIER = parser.getDouble("TrainingCampExpMultiplier", 1.0);
		TRAINING_CAMP_SP_MULTIPLIER = parser.getDouble("TrainingCampSpMultiplier", 1.0);

		// Load GameAssistant config file (if exists)
		parser.LoadConfig(GAME_ASSISTANT_CONFIG_FILE);
		GAME_ASSISTANT_ENABLED = parser.getBoolean("GameAssistantEnabled", false);

		// Load General config file (if exists)
		parser.LoadConfig(GENERAL_CONFIG_FILE);
		DEFAULT_ACCESS_LEVEL = parser.getInt("DefaultAccessLevel", 0);
		SERVER_GMONLY = parser.getBoolean("ServerGMOnly", false);
		GM_HERO_AURA = parser.getBoolean("GMHeroAura", false);
		GM_STARTUP_BUILDER_HIDE = parser.getBoolean("GMStartupBuilderHide", false);
		GM_STARTUP_INVULNERABLE = parser.getBoolean("GMStartupInvulnerable", false);
		GM_STARTUP_INVISIBLE = parser.getBoolean("GMStartupInvisible", false);
		GM_STARTUP_SILENCE = parser.getBoolean("GMStartupSilence", false);
		GM_STARTUP_AUTO_LIST = parser.getBoolean("GMStartupAutoList", false);
		GM_STARTUP_DIET_MODE = parser.getBoolean("GMStartupDietMode", false);
		GM_ITEM_RESTRICTION = parser.getBoolean("GMItemRestriction", true);
		GM_SKILL_RESTRICTION = parser.getBoolean("GMSkillRestriction", true);
		GM_TRADE_RESTRICTED_ITEMS = parser.getBoolean("GMTradeRestrictedItems", false);
		GM_RESTART_FIGHTING = parser.getBoolean("GMRestartFighting", true);
		GM_ANNOUNCER_NAME = parser.getBoolean("GMShowAnnouncerName", false);
		GM_GIVE_SPECIAL_SKILLS = parser.getBoolean("GMGiveSpecialSkills", false);
		GM_GIVE_SPECIAL_AURA_SKILLS = parser.getBoolean("GMGiveSpecialAuraSkills", false);
		GM_DEBUG_HTML_PATHS = parser.getBoolean("GMDebugHtmlPaths", true);
		USE_SUPER_HASTE_AS_GM_SPEED = parser.getBoolean("UseSuperHasteAsGMSpeed", false);
		LOG_CHAT = parser.getBoolean("LogChat", false);
		LOG_AUTO_ANNOUNCEMENTS = parser.getBoolean("LogAutoAnnouncements", false);
		LOG_ITEMS = parser.getBoolean("LogItems", false);
		LOG_ITEMS_SMALL_LOG = parser.getBoolean("LogItemsSmallLog", false);
		LOG_ITEMS_IDS_ONLY = parser.getBoolean("LogItemsIdsOnly", false);
		LOG_ITEMS_IDS_LIST = parser.GetIntList("LogItemsIdsList").ToImmutableSortedSet();
		LOG_ITEM_ENCHANTS = parser.getBoolean("LogItemEnchants", false);
		LOG_SKILL_ENCHANTS = parser.getBoolean("LogSkillEnchants", false);
		GMAUDIT = parser.getBoolean("GMAudit", false);
		SKILL_CHECK_ENABLE = parser.getBoolean("SkillCheckEnable", false);
		SKILL_CHECK_REMOVE = parser.getBoolean("SkillCheckRemove", false);
		SKILL_CHECK_GM = parser.getBoolean("SkillCheckGM", true);
		HTML_ACTION_CACHE_DEBUG = parser.getBoolean("HtmlActionCacheDebug", false);
		DEVELOPER = parser.getBoolean("Developer", false);
		ALT_DEV_NO_QUESTS = parser.getBoolean("AltDevNoQuests", false);
		ALT_DEV_NO_SPAWNS = parser.getBoolean("AltDevNoSpawns", false);
		ALT_DEV_SHOW_QUESTS_LOAD_IN_LOGS = parser.getBoolean("AltDevShowQuestsLoadInLogs", false);
		ALT_DEV_SHOW_SCRIPTS_LOAD_IN_LOGS = parser.getBoolean("AltDevShowScriptsLoadInLogs", false);
		DEBUG_CLIENT_PACKETS = parser.getBoolean("DebugClientPackets", false);
		DEBUG_EX_CLIENT_PACKETS = parser.getBoolean("DebugExClientPackets", false);
		DEBUG_SERVER_PACKETS = parser.getBoolean("DebugServerPackets", false);
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
		LAZY_ITEMS_UPDATE = parser.getBoolean("LazyItemsUpdate", false);
		UPDATE_ITEMS_ON_CHAR_STORE = parser.getBoolean("UpdateItemsOnCharStore", false);
		DESTROY_DROPPED_PLAYER_ITEM = parser.getBoolean("DestroyPlayerDroppedItem", false);
		DESTROY_EQUIPABLE_PLAYER_ITEM = parser.getBoolean("DestroyEquipableItem", false);
		DESTROY_ALL_ITEMS = parser.getBoolean("DestroyAllItems", false);
		SAVE_DROPPED_ITEM = parser.getBoolean("SaveDroppedItem", false);
		EMPTY_DROPPED_ITEM_TABLE_AFTER_LOAD = parser.getBoolean("EmptyDroppedItemTableAfterLoad", false);
		SAVE_DROPPED_ITEM_INTERVAL = parser.getInt("SaveDroppedItemInterval", 60) * 60000;
		CLEAR_DROPPED_ITEM_TABLE = parser.getBoolean("ClearDroppedItemTable", false);
		ORDER_QUEST_LIST_BY_QUESTID = parser.getBoolean("OrderQuestListByQuestId", true);
		AUTODELETE_INVALID_QUEST_DATA = parser.getBoolean("AutoDeleteInvalidQuestData", false);
		ENABLE_STORY_QUEST_BUFF_REWARD = parser.getBoolean("StoryQuestRewardBuff", true);
		MULTIPLE_ITEM_DROP = parser.getBoolean("MultipleItemDrop", true);
		HTM_CACHE = parser.getBoolean("HtmCache", true);
		CHECK_HTML_ENCODING = parser.getBoolean("CheckHtmlEncoding", true);
		MIN_NPC_ANIMATION = parser.getInt("MinNpcAnimation", 5);
		MAX_NPC_ANIMATION = parser.getInt("MaxNpcAnimation", 60);
		MIN_MONSTER_ANIMATION = parser.getInt("MinMonsterAnimation", 5);
		MAX_MONSTER_ANIMATION = parser.getInt("MaxMonsterAnimation", 60);
		GRIDS_ALWAYS_ON = parser.getBoolean("GridsAlwaysOn", false);
		GRID_NEIGHBOR_TURNON_TIME = parser.getInt("GridNeighborTurnOnTime", 1);
		GRID_NEIGHBOR_TURNOFF_TIME = parser.getInt("GridNeighborTurnOffTime", 90);
		CORRECT_PRICES = parser.getBoolean("CorrectPrices", true);
		ENABLE_FALLING_DAMAGE = parser.getBoolean("EnableFallingDamage", true);
		PEACE_ZONE_MODE = parser.getInt("PeaceZoneMode", 0);
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
		RESTORE_PLAYER_INSTANCE = parser.getBoolean("RestorePlayerInstance", false);
		EJECT_DEAD_PLAYER_TIME = parser.getInt("EjectDeadPlayerTime", 1);
		ALLOW_RACE = parser.getBoolean("AllowRace", true);
		ALLOW_WATER = parser.getBoolean("AllowWater", true);
		ALLOW_FISHING = parser.getBoolean("AllowFishing", true);
		ALLOW_MANOR = parser.getBoolean("AllowManor", true);
		ALLOW_BOAT = parser.getBoolean("AllowBoat", true);
		BOAT_BROADCAST_RADIUS = parser.getInt("BoatBroadcastRadius", 20000);
		ALLOW_CURSED_WEAPONS = parser.getBoolean("AllowCursedWeapons", true);
		SERVER_NEWS = parser.getBoolean("ShowServerNews", false);
		ENABLE_COMMUNITY_BOARD = parser.getBoolean("EnableCommunityBoard", true);
		BBS_DEFAULT = parser.getString("BBSDefault", "_bbshome");
		USE_SAY_FILTER = parser.getBoolean("UseChatFilter", false);
		CHAT_FILTER_CHARS = parser.getString("ChatFilterChars", "^_^");
		BAN_CHAT_CHANNELS = parser.GetEnumList("BanChatChannels", ';', ChatType.GENERAL, ChatType.SHOUT,
			ChatType.WORLD, ChatType.TRADE, ChatType.HERO_VOICE).ToImmutableSortedSet();

		WORLD_CHAT_MIN_LEVEL = parser.getInt("WorldChatMinLevel", 95);
		WORLD_CHAT_POINTS_PER_DAY = parser.getInt("WorldChatPointsPerDay", 10);
		WORLD_CHAT_INTERVAL = parser.GetTimeSpan("WorldChatInterval", TimeSpan.FromSeconds(20));
		ALT_MANOR_REFRESH_TIME = parser.getInt("AltManorRefreshTime", 20);
		ALT_MANOR_REFRESH_MIN = parser.getInt("AltManorRefreshMin", 0);
		ALT_MANOR_APPROVE_TIME = parser.getInt("AltManorApproveTime", 4);
		ALT_MANOR_APPROVE_MIN = parser.getInt("AltManorApproveMin", 30);
		ALT_MANOR_MAINTENANCE_MIN = parser.getInt("AltManorMaintenanceMin", 6);
		ALT_MANOR_SAVE_ALL_ACTIONS = parser.getBoolean("AltManorSaveAllActions", false);
		ALT_MANOR_SAVE_PERIOD_RATE = parser.getInt("AltManorSavePeriodRate", 2);
		ALT_ITEM_AUCTION_ENABLED = parser.getBoolean("AltItemAuctionEnabled", true);
		ALT_ITEM_AUCTION_EXPIRED_AFTER = parser.getInt("AltItemAuctionExpiredAfter", 14);
		ALT_ITEM_AUCTION_TIME_EXTENDS_ON_BID = parser.getInt("AltItemAuctionTimeExtendsOnBid", 0) * 1000;
		DEFAULT_PUNISH = parser.GetEnum("DefaultPunish", IllegalActionPunishmentType.KICK);
		DEFAULT_PUNISH_PARAM = parser.getLong("DefaultPunishParam", 0);
		if (DEFAULT_PUNISH_PARAM == 0)
		{
			DEFAULT_PUNISH_PARAM = 3155695200L; // One hundred years in seconds.
		}

		ONLY_GM_ITEMS_FREE = parser.getBoolean("OnlyGMItemsFree", true);
		JAIL_IS_PVP = parser.getBoolean("JailIsPvp", false);
		JAIL_DISABLE_CHAT = parser.getBoolean("JailDisableChat", true);
		JAIL_DISABLE_TRANSACTION = parser.getBoolean("JailDisableTransaction", false);
		CUSTOM_NPC_DATA = parser.getBoolean("CustomNpcData", false);
		CUSTOM_TELEPORT_TABLE = parser.getBoolean("CustomTeleportTable", false);
		CUSTOM_SKILLS_LOAD = parser.getBoolean("CustomSkillsLoad", false);
		CUSTOM_ITEMS_LOAD = parser.getBoolean("CustomItemsLoad", false);
		CUSTOM_MULTISELL_LOAD = parser.getBoolean("CustomMultisellLoad", false);
		CUSTOM_BUYLIST_LOAD = parser.getBoolean("CustomBuyListLoad", false);
		BOOKMARK_CONSUME_ITEM_ID = parser.getInt("BookmarkConsumeItemId", -1);
		ALT_BIRTHDAY_GIFT = parser.getInt("AltBirthdayGift", 72078);
		ALT_BIRTHDAY_MAIL_SUBJECT = parser.getString("AltBirthdayMailSubject", "Happy Birthday!");
		ALT_BIRTHDAY_MAIL_TEXT = parser.getString("AltBirthdayMailText",
			"Hello Adventurer!! Seeing as you're one year older now, I thought I would send you some birthday cheer :) Please find your birthday pack attached. May these gifts bring you joy and happiness on this very special day." +
			Environment.NewLine + Environment.NewLine + "Sincerely, Alegria");
		BOTREPORT_ENABLE = parser.getBoolean("EnableBotReportButton", false);
		BOTREPORT_RESETPOINT_HOUR = parser.GetTime("BotReportPointsResetHour");
		BOTREPORT_REPORT_DELAY = parser.getInt("BotReportDelay", 30) * 60000;
		BOTREPORT_ALLOW_REPORTS_FROM_SAME_CLAN_MEMBERS = parser.getBoolean("AllowReportsFromSameClanMembers", false);
		ENABLE_AUTO_PLAY = parser.getBoolean("EnableAutoPlay", true);
		ENABLE_AUTO_POTION = parser.getBoolean("EnableAutoPotion", true);
		ENABLE_AUTO_PET_POTION = parser.getBoolean("EnableAutoPetPotion", true);
		ENABLE_AUTO_SKILL = parser.getBoolean("EnableAutoSkill", true);
		ENABLE_AUTO_ITEM = parser.getBoolean("EnableAutoItem", true);
		AUTO_PLAY_ATTACK_ACTION = parser.getBoolean("AutoPlayAttackAction", true);
		RESUME_AUTO_PLAY = parser.getBoolean("ResumeAutoPlay", false);
		ENABLE_AUTO_ASSIST = parser.getBoolean("AssistLeader", false);
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
		ANNOUNCE_MAMMON_SPAWN = parser.getBoolean("AnnounceMammonSpawn", false);
		ALT_MOB_AGRO_IN_PEACEZONE = parser.getBoolean("AltMobAgroInPeaceZone", true);
		ALT_ATTACKABLE_NPCS = parser.getBoolean("AltAttackableNpcs", true);
		ALT_GAME_VIEWNPC = parser.getBoolean("AltGameViewNpc", false);
		SHOW_NPC_LEVEL = parser.getBoolean("ShowNpcLevel", false);
		SHOW_NPC_AGGRESSION = parser.getBoolean("ShowNpcAggression", false);
		ATTACKABLES_CAMP_PLAYER_CORPSES = parser.getBoolean("AttackablesCampPlayerCorpses", false);
		SHOW_CREST_WITHOUT_QUEST = parser.getBoolean("ShowCrestWithoutQuest", false);
		ENABLE_RANDOM_ENCHANT_EFFECT = parser.getBoolean("EnableRandomEnchantEffect", false);
		MIN_NPC_LEVEL_DMG_PENALTY = parser.getInt("MinNPCLevelForDmgPenalty", 78);
		NPC_DMG_PENALTY = parser.GetDoubleList("DmgPenaltyForLvLDifferences", ',', 0.7, 0.6, 0.6, 0.55);
		NPC_CRIT_DMG_PENALTY = parser.GetDoubleList("CritDmgPenaltyForLvLDifferences", ',', 0.75, 0.65, 0.6, 0.58);
		NPC_SKILL_DMG_PENALTY = parser.GetDoubleList("SkillDmgPenaltyForLvLDifferences", ',', 0.8, 0.7, 0.65, 0.62);
		MIN_NPC_LEVEL_MAGIC_PENALTY = parser.getInt("MinNPCLevelForMagicPenalty", 78);
		NPC_SKILL_CHANCE_PENALTY = parser.GetDoubleList("SkillChancePenaltyForLvLDifferences", ',', 2.5, 3.0, 3.25, 3.5);
		DEFAULT_CORPSE_TIME = parser.getInt("DefaultCorpseTime", 7);
		SPOILED_CORPSE_EXTEND_TIME = parser.getInt("SpoiledCorpseExtendTime", 10);
		CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY =
			parser.getInt("CorpseConsumeSkillAllowedTimeBeforeDecay", 2000);
		MAX_DRIFT_RANGE = parser.getInt("MaxDriftRange", 300);
		AGGRO_DISTANCE_CHECK_ENABLED = parser.getBoolean("AggroDistanceCheckEnabled", true);
		AGGRO_DISTANCE_CHECK_RANGE = parser.getInt("AggroDistanceCheckRange", 1500);
		AGGRO_DISTANCE_CHECK_RAIDS = parser.getBoolean("AggroDistanceCheckRaids", false);
		AGGRO_DISTANCE_CHECK_RAID_RANGE = parser.getInt("AggroDistanceCheckRaidRange", 3000);
		AGGRO_DISTANCE_CHECK_INSTANCES = parser.getBoolean("AggroDistanceCheckInstances", false);
		AGGRO_DISTANCE_CHECK_RESTORE_LIFE = parser.getBoolean("AggroDistanceCheckRestoreLife", true);
		GUARD_ATTACK_AGGRO_MOB = parser.getBoolean("GuardAttackAggroMob", false);
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
		FORCE_DELETE_MINIONS = parser.getBoolean("ForceDeleteMinions", false);
		RAID_DISABLE_CURSE = parser.getBoolean("DisableRaidCurse", false);
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
		RATE_QUEST_REWARD_USE_MULTIPLIERS = parser.getBoolean("UseQuestRewardMultipliers", false);
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
		BOSS_DROP_ENABLED = parser.getBoolean("BossDropEnable", false);
		BOSS_DROP_MIN_LEVEL = parser.getInt("BossDropMinLevel", 40);
		BOSS_DROP_MAX_LEVEL = parser.getInt("BossDropMaxLevel", 999);
		BOSS_DROP_LIST = GetDropList(parser, "BossDropList");
		LCOIN_DROP_ENABLED = parser.getBoolean("LCoinDropEnable", false);
		LCOIN_DROP_CHANCE = parser.getDouble("LCoinDropChance", 15.0);
		LCOIN_MIN_MOB_LEVEL = parser.getInt("LCoinMinimumMonsterLevel", 40);
		LCOIN_MIN_QUANTITY = parser.getInt("LCoinMinDropQuantity", 1);
		LCOIN_MAX_QUANTITY = parser.getInt("LCoinMaxDropQuantity", 5);

		// Load PvP config file (if exists)
		parser.LoadConfig(PVP_CONFIG_FILE);
		KARMA_DROP_GM = parser.getBoolean("CanGMDropEquipment", false);
		KARMA_PK_LIMIT = parser.getInt("MinimumPKRequiredToDrop", 4);
		KARMA_NONDROPPABLE_PET_ITEMS = parser.GetIntList("ListOfPetItems", ',', 2375, 3500, 3501, 3502, 4422, 4423,
			4424, 4425, 6648, 6649, 6650, 9882).ToImmutableSortedSet();

		KARMA_NONDROPPABLE_ITEMS = parser.GetIntList("ListOfNonDroppableItems", ',', 57, 1147, 425, 1146, 461, 10, 2368,
			7, 6, 2370, 2369, 6842, 6611, 6612, 6613, 6614, 6615, 6616, 6617, 6618, 6619, 6620, 6621, 7694, 8181, 5575,
			7694, 9388, 9389, 9390).ToImmutableSortedSet();
		ANTIFEED_ENABLE = parser.getBoolean("AntiFeedEnable", false);
		ANTIFEED_DUALBOX = parser.getBoolean("AntiFeedDualbox", true);
		ANTIFEED_DISCONNECTED_AS_DUALBOX = parser.getBoolean("AntiFeedDisconnectedAsDualbox", true);
		ANTIFEED_INTERVAL = parser.getInt("AntiFeedInterval", 120) * 1000;
		VAMPIRIC_ATTACK_AFFECTS_PVP = parser.getBoolean("VampiricAttackAffectsPvP", false);
		MP_VAMPIRIC_ATTACK_AFFECTS_PVP = parser.getBoolean("MpVampiricAttackAffectsPvP", false);
		PVP_NORMAL_TIME = parser.getInt("PvPVsNormalTime", 120000);
		PVP_PVP_TIME = parser.getInt("PvPVsPvPTime", 60000);
		MAX_REPUTATION = parser.getInt("MaxReputation", 500);
		REPUTATION_INCREASE = parser.getInt("ReputationIncrease", 100);

		// Load Olympiad config file (if exists)
		parser.LoadConfig(OLYMPIAD_CONFIG_FILE);
		OLYMPIAD_ENABLED = parser.getBoolean("OlympiadEnabled", true);
		ALT_OLY_START_TIME = parser.getInt("AltOlyStartTime", 20);
		ALT_OLY_MIN = parser.getInt("AltOlyMin", 0);
		ALT_OLY_CPERIOD = parser.getLong("AltOlyCPeriod", 14400000);
		ALT_OLY_BATTLE = parser.getLong("AltOlyBattle", 300000);
		ALT_OLY_WPERIOD = parser.getLong("AltOlyWPeriod", 604800000);
		ALT_OLY_VPERIOD = parser.getLong("AltOlyVPeriod", 86400000);
		ALT_OLY_START_POINTS = parser.getInt("AltOlyStartPoints", 10);
		ALT_OLY_WEEKLY_POINTS = parser.getInt("AltOlyWeeklyPoints", 10);
		ALT_OLY_CLASSED = parser.getInt("AltOlyClassedParticipants", 10);
		ALT_OLY_NONCLASSED = parser.getInt("AltOlyNonClassedParticipants", 20);
		ALT_OLY_WINNER_REWARD = parser.GetIdValueMap<int>("AltOlyWinReward");
		ALT_OLY_LOSER_REWARD = parser.GetIdValueMap<int>("AltOlyLoserReward");
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
		ALT_OLY_LOG_FIGHTS = parser.getBoolean("AltOlyLogFights", false);
		ALT_OLY_SHOW_MONTHLY_WINNERS = parser.getBoolean("AltOlyShowMonthlyWinners", true);
		ALT_OLY_ANNOUNCE_GAMES = parser.getBoolean("AltOlyAnnounceGames", true);
		LIST_OLY_RESTRICTED_ITEMS = parser.GetIntList("AltOlyRestrictedItems").ToImmutableSortedSet();
		ALT_OLY_WEAPON_ENCHANT_LIMIT = parser.getInt("AltOlyWeaponEnchantLimit", -1);
		ALT_OLY_ARMOR_ENCHANT_LIMIT = parser.getInt("AltOlyArmorEnchantLimit", -1);
		ALT_OLY_WAIT_TIME = parser.getInt("AltOlyWaitTime", 60);
		ALT_OLY_PERIOD = parser.getString("AltOlyPeriod", "MONTH");
		ALT_OLY_PERIOD_MULTIPLIER = parser.getInt("AltOlyPeriodMultiplier", 1);
		ALT_OLY_COMPETITION_DAYS = parser.GetIntList("AltOlyCompetitionDays").ToImmutableSortedSet();

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
		GEODATA_PATH = Path.Combine(DATAPACK_ROOT_PATH, parser.getString("GeoDataPath", "geodata"));
		PATHNODE_PATH = Path.Combine(DATAPACK_ROOT_PATH, parser.getString("PathnodePath", "pathnode"));
		PATHFINDING = parser.getInt("PathFinding", 0);
		PATHFIND_BUFFERS = parser.getString("PathFindBuffers", "100x6;128x6;192x6;256x4;320x4;384x4;500x2");
		LOW_WEIGHT = parser.getFloat("LowWeight", 0.5f);
		MEDIUM_WEIGHT = parser.getFloat("MediumWeight", 2);
		HIGH_WEIGHT = parser.getFloat("HighWeight", 3);
		ADVANCED_DIAGONAL_STRATEGY = parser.getBoolean("AdvancedDiagonalStrategy", true);
		DIAGONAL_WEIGHT = parser.getFloat("DiagonalWeight", 0.707f);
		MAX_POSTFILTER_PASSES = parser.getInt("MaxPostfilterPasses", 3);
		DEBUG_PATH = parser.getBoolean("DebugPath", false);

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
		AUTO_POTIONS_ENABLED = parser.getBoolean("AutoPotionsEnabled", false);
		AUTO_POTIONS_IN_OLYMPIAD = parser.getBoolean("AutoPotionsInOlympiad", false);
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
		BANKING_SYSTEM_ENABLED = parser.getBoolean("BankingEnabled", false);
		BANKING_SYSTEM_GOLDBARS = parser.getInt("BankingGoldbarCount", 1);
		BANKING_SYSTEM_ADENA = parser.getInt("BankingAdenaCount", 500000000);

		// Load Boss Announcements config file (if exists)
		parser.LoadConfig(CUSTOM_BOSS_ANNOUNCEMENTS_CONFIG_FILE);
		RAIDBOSS_SPAWN_ANNOUNCEMENTS = parser.getBoolean("RaidBossSpawnAnnouncements", false);
		RAIDBOSS_DEFEAT_ANNOUNCEMENTS = parser.getBoolean("RaidBossDefeatAnnouncements", false);
		RAIDBOSS_INSTANCE_ANNOUNCEMENTS = parser.getBoolean("RaidBossInstanceAnnouncements", false);
		GRANDBOSS_SPAWN_ANNOUNCEMENTS = parser.getBoolean("GrandBossSpawnAnnouncements", false);
		GRANDBOSS_DEFEAT_ANNOUNCEMENTS = parser.getBoolean("GrandBossDefeatAnnouncements", false);
		GRANDBOSS_INSTANCE_ANNOUNCEMENTS = parser.getBoolean("GrandBossInstanceAnnouncements", false);

		// Load BoostNpcStats config file (if exists)
		parser.LoadConfig(CUSTOM_NPC_STAT_MULTIPLIERS_CONFIG_FILE);
		ENABLE_NPC_STAT_MULTIPLIERS = parser.getBoolean("EnableNpcStatMultipliers", false);
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
		CHAMPION_ENABLE = parser.getBoolean("ChampionEnable", false);
		CHAMPION_PASSIVE = parser.getBoolean("ChampionPassive", false);
		CHAMPION_FREQUENCY = parser.getInt("ChampionFrequency", 0);
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
		CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE = parser.getInt("ChampionRewardLowerLvlItemChance", 0);
		CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE = parser.getInt("ChampionRewardHigherLvlItemChance", 0);
		CHAMPION_REWARD_ITEMS = parser.GetIdValueMap<int>("ChampionRewardItems");
		CHAMPION_ENABLE_VITALITY = parser.getBoolean("ChampionEnableVitality", false);
		CHAMPION_ENABLE_IN_INSTANCES = parser.getBoolean("ChampionEnableInInstances", false);

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
		CUSTOM_CB_ENABLED = parser.getBoolean("CustomCommunityBoard", false);
		COMMUNITYBOARD_CURRENCY = parser.getInt("CommunityCurrencyId", 57);
		COMMUNITYBOARD_ENABLE_MULTISELLS = parser.getBoolean("CommunityEnableMultisells", true);
		COMMUNITYBOARD_ENABLE_TELEPORTS = parser.getBoolean("CommunityEnableTeleports", true);
		COMMUNITYBOARD_ENABLE_BUFFS = parser.getBoolean("CommunityEnableBuffs", true);
		COMMUNITYBOARD_ENABLE_HEAL = parser.getBoolean("CommunityEnableHeal", true);
		COMMUNITYBOARD_ENABLE_DELEVEL = parser.getBoolean("CommunityEnableDelevel", false);
		COMMUNITYBOARD_TELEPORT_PRICE = parser.getInt("CommunityTeleportPrice", 0);
		COMMUNITYBOARD_BUFF_PRICE = parser.getInt("CommunityBuffPrice", 0);
		COMMUNITYBOARD_HEAL_PRICE = parser.getInt("CommunityHealPrice", 0);
		COMMUNITYBOARD_DELEVEL_PRICE = parser.getInt("CommunityDelevelPrice", 0);
		COMMUNITYBOARD_KARMA_DISABLED = parser.getBoolean("CommunityKarmaDisabled", true);
		COMMUNITYBOARD_CAST_ANIMATIONS = parser.getBoolean("CommunityCastAnimations", false);
		COMMUNITY_PREMIUM_SYSTEM_ENABLED = parser.getBoolean("CommunityPremiumSystem", false);
		COMMUNITY_PREMIUM_COIN_ID = parser.getInt("CommunityPremiumBuyCoinId", 57);
		COMMUNITY_PREMIUM_PRICE_PER_DAY = parser.getInt("CommunityPremiumPricePerDay", 1000000);
		COMMUNITY_AVAILABLE_BUFFS = parser.GetIntList("CommunityAvailableBuffs").ToImmutableSortedSet();
		COMMUNITY_AVAILABLE_TELEPORTS = GetLocations(parser, "CommunityTeleportList");

		// Load CustomDepositableItems config file (if exists)
		parser.LoadConfig(CUSTOM_CUSTOM_DEPOSITABLE_ITEMS_CONFIG_FILE);
		CUSTOM_DEPOSITABLE_ENABLED = parser.getBoolean("CustomDepositableEnabled", false);
		CUSTOM_DEPOSITABLE_QUEST_ITEMS = parser.getBoolean("DepositableQuestItems", false);

		// Load CustomMailManager config file (if exists)
		parser.LoadConfig(CUSTOM_CUSTOM_MAIL_MANAGER_CONFIG_FILE);
		CUSTOM_MAIL_MANAGER_ENABLED = parser.getBoolean("CustomMailManagerEnabled", false);
		CUSTOM_MAIL_MANAGER_DELAY = parser.getInt("DatabaseQueryDelay", 30) * 1000;

		// Load DelevelManager config file (if exists)
		parser.LoadConfig(CUSTOM_DELEVEL_MANAGER_CONFIG_FILE);
		DELEVEL_MANAGER_ENABLED = parser.getBoolean("Enabled", false);
		DELEVEL_MANAGER_NPCID = parser.getInt("NpcId", 1002000);
		DELEVEL_MANAGER_ITEMID = parser.getInt("RequiredItemId", 4356);
		DELEVEL_MANAGER_ITEMCOUNT = parser.getInt("RequiredItemCount", 2);
		DELEVEL_MANAGER_MINIMUM_DELEVEL = parser.getInt("MimimumDelevel", 20);

		// Load DualboxCheck config file (if exists)
		parser.LoadConfig(CUSTOM_DUALBOX_CHECK_CONFIG_FILE);
		DUALBOX_CHECK_MAX_PLAYERS_PER_IP = parser.getInt("DualboxCheckMaxPlayersPerIP", 0);
		DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP = parser.getInt("DualboxCheckMaxOlympiadParticipantsPerIP", 0);
		DUALBOX_CHECK_MAX_L2EVENT_PARTICIPANTS_PER_IP = parser.getInt("DualboxCheckMaxL2EventParticipantsPerIP", 0);
		DUALBOX_COUNT_OFFLINE_TRADERS = parser.getBoolean("DualboxCountOfflineTraders", false);
		// DUALBOX_CHECK_WHITELIST = parser.getString("DualboxCheckWhitelist", "127.0.0.1,0"); // TODO: implement 

		// Load FactionSystem config file (if exists)
		parser.LoadConfig(CUSTOM_FACTION_SYSTEM_CONFIG_FILE);
		FACTION_SYSTEM_ENABLED = parser.getBoolean("EnableFactionSystem", false);
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
		FACTION_AUTO_NOBLESS = parser.getBoolean("FactionAutoNobless", false);
		FACTION_SPECIFIC_CHAT = parser.getBoolean("EnableFactionChat", true);
		FACTION_BALANCE_ONLINE_PLAYERS = parser.getBoolean("BalanceOnlinePlayers", true);
		FACTION_BALANCE_PLAYER_EXCEED_LIMIT = parser.getInt("BalancePlayerExceedLimit", 20);

		// Load FakePlayers config file (if exists)
		parser.LoadConfig(CUSTOM_FAKE_PLAYERS_CONFIG_FILE);
		FAKE_PLAYERS_ENABLED = parser.getBoolean("EnableFakePlayers", false);
		FAKE_PLAYER_CHAT = parser.getBoolean("FakePlayerChat", false);
		FAKE_PLAYER_USE_SHOTS = parser.getBoolean("FakePlayerUseShots", false);
		FAKE_PLAYER_KILL_PVP = parser.getBoolean("FakePlayerKillsRewardPvP", false);
		FAKE_PLAYER_KILL_KARMA = parser.getBoolean("FakePlayerUnflaggedKillsKarma", false);
		FAKE_PLAYER_AUTO_ATTACKABLE = parser.getBoolean("FakePlayerAutoAttackable", false);
		FAKE_PLAYER_AGGRO_MONSTERS = parser.getBoolean("FakePlayerAggroMonsters", false);
		FAKE_PLAYER_AGGRO_PLAYERS = parser.getBoolean("FakePlayerAggroPlayers", false);
		FAKE_PLAYER_AGGRO_FPC = parser.getBoolean("FakePlayerAggroFPC", false);
		FAKE_PLAYER_CAN_DROP_ITEMS = parser.getBoolean("FakePlayerCanDropItems", false);
		FAKE_PLAYER_CAN_PICKUP = parser.getBoolean("FakePlayerCanPickup", false);

		// Load FindPvP config file (if exists)
		parser.LoadConfig(CUSTOM_FIND_PVP_CONFIG_FILE);
		ENABLE_FIND_PVP = parser.getBoolean("EnableFindPvP", false);

		// Load MerchantZeroSellPrice config file (if exists)
		parser.LoadConfig(CUSTOM_MERCHANT_ZERO_SELL_PRICE_CONFIG_FILE);
		MERCHANT_ZERO_SELL_PRICE = parser.getBoolean("MerchantZeroSellPrice", false);

		// Load MultilingualSupport config file (if exists)
		parser.LoadConfig(CUSTOM_MULTILANGUAL_SUPPORT_CONFIG_FILE);
		MULTILANG_DEFAULT = parser.getString("MultiLangDefault", "en").ToLower();
		MULTILANG_ENABLE = parser.getBoolean("MultiLangEnable", false);
		if (MULTILANG_ENABLE)
		{
			CHECK_HTML_ENCODING = false;
		}

		MULTILANG_ALLOWED = parser.GetStringList("MultiLangAllowed", ',', MULTILANG_DEFAULT);
		if (!MULTILANG_ALLOWED.Contains(MULTILANG_DEFAULT))
		{
			LOGGER.Error(
				$"Default language missing in entry 'MultiLangAllowed' in configuration file '{parser.FilePath}'");
		}

		MULTILANG_VOICED_ALLOW = parser.getBoolean("MultiLangVoiceCommand", true);

		// Load NoblessMaster config file (if exists)
		parser.LoadConfig(CUSTOM_NOBLESS_MASTER_CONFIG_FILE);
		NOBLESS_MASTER_ENABLED = parser.getBoolean("Enabled", false);
		NOBLESS_MASTER_NPCID = parser.getInt("NpcId", 1003000);
		NOBLESS_MASTER_LEVEL_REQUIREMENT = parser.getInt("LevelRequirement", 80);
		NOBLESS_MASTER_REWARD_TIARA = parser.getBoolean("RewardTiara", false);

		// Load OfflinePlay config file (if exists)
		parser.LoadConfig(CUSTOM_OFFLINE_PLAY_CONFIG_FILE);
		ENABLE_OFFLINE_PLAY_COMMAND = parser.getBoolean("EnableOfflinePlayCommand", false);
		OFFLINE_PLAY_PREMIUM = parser.getBoolean("OfflinePlayPremium", false);
		OFFLINE_PLAY_LOGOUT_ON_DEATH = parser.getBoolean("OfflinePlayLogoutOnDeath", true);
		OFFLINE_PLAY_LOGIN_MESSAGE = parser.getString("OfflinePlayLoginMessage", "");
		OFFLINE_PLAY_SET_NAME_COLOR = parser.getBoolean("OfflinePlaySetNameColor", false);
		OFFLINE_PLAY_NAME_COLOR = parser.GetColor("OfflinePlayNameColor", new Color(0x808080));
		OFFLINE_PLAY_ABNORMAL_EFFECTS = parser.GetEnumList<AbnormalVisualEffect>("OfflinePlayAbnormalEffect");

		// Load OfflineTrade config file (if exists)
		parser.LoadConfig(CUSTOM_OFFLINE_TRADE_CONFIG_FILE);
		OFFLINE_TRADE_ENABLE = parser.getBoolean("OfflineTradeEnable", false);
		OFFLINE_CRAFT_ENABLE = parser.getBoolean("OfflineCraftEnable", false);
		OFFLINE_MODE_IN_PEACE_ZONE = parser.getBoolean("OfflineModeInPeaceZone", false);
		OFFLINE_MODE_NO_DAMAGE = parser.getBoolean("OfflineModeNoDamage", false);
		OFFLINE_SET_NAME_COLOR = parser.getBoolean("OfflineSetNameColor", false);
		OFFLINE_NAME_COLOR = parser.GetColor("OfflineNameColor", new Color(0x808080));
		OFFLINE_FAME = parser.getBoolean("OfflineFame", true);
		RESTORE_OFFLINERS = parser.getBoolean("RestoreOffliners", false);
		OFFLINE_MAX_DAYS = parser.getInt("OfflineMaxDays", 10);
		OFFLINE_DISCONNECT_FINISHED = parser.getBoolean("OfflineDisconnectFinished", true);
		OFFLINE_DISCONNECT_SAME_ACCOUNT = parser.getBoolean("OfflineDisconnectSameAccount", false);
		STORE_OFFLINE_TRADE_IN_REALTIME = parser.getBoolean("StoreOfflineTradeInRealtime", true);
		ENABLE_OFFLINE_COMMAND = parser.getBoolean("EnableOfflineCommand", true);
		OFFLINE_ABNORMAL_EFFECTS = parser.GetEnumList<AbnormalVisualEffect>("OfflineAbnormalEffect");

		// Load OnlineInfo config file (if exists)
		parser.LoadConfig(CUSTOM_ONLINE_INFO_CONFIG_FILE);
		ENABLE_ONLINE_COMMAND = parser.getBoolean("EnableOnlineCommand", false);

		// Load PasswordChange config file (if exists)
		parser.LoadConfig(CUSTOM_PASSWORD_CHANGE_CONFIG_FILE);
		ALLOW_CHANGE_PASSWORD = parser.getBoolean("AllowChangePassword", false);

		parser.LoadConfig(CUSTOM_VIP_CONFIG_FILE);
		VIP_SYSTEM_ENABLED = parser.getBoolean("VipEnabled", false);
		if (VIP_SYSTEM_ENABLED)
		{
			VIP_SYSTEM_PRIME_AFFECT = parser.getBoolean("PrimeAffectPoints", false);
			VIP_SYSTEM_L_SHOP_AFFECT = parser.getBoolean("LShopAffectPoints", false);
			VIP_SYSTEM_MAX_TIER = parser.getInt("MaxVipLevel", 7);
			if (VIP_SYSTEM_MAX_TIER > 10)
			{
				VIP_SYSTEM_MAX_TIER = 10;
			}
		}

		// Load PremiumSystem config file (if exists)
		parser.LoadConfig(CUSTOM_PREMIUM_SYSTEM_CONFIG_FILE);
		PREMIUM_SYSTEM_ENABLED = parser.getBoolean("EnablePremiumSystem", false);
		PC_CAFE_ENABLED = parser.getBoolean("PcCafeEnabled", false);
		PC_CAFE_ONLY_PREMIUM = parser.getBoolean("PcCafeOnlyPremium", false);
		PC_CAFE_ONLY_VIP = parser.getBoolean("PcCafeOnlyVip", false);
		PC_CAFE_RETAIL_LIKE = parser.getBoolean("PcCafeRetailLike", true);
		PC_CAFE_MAX_POINTS = parser.getInt("MaxPcCafePoints", 200000);
		if (PC_CAFE_MAX_POINTS < 0)
		{
			PC_CAFE_MAX_POINTS = 0;
		}

		PC_CAFE_ENABLE_DOUBLE_POINTS = parser.getBoolean("DoublingAcquisitionPoints", false);
		PC_CAFE_DOUBLE_POINTS_CHANCE = parser.getInt("DoublingAcquisitionPointsChance", 1);
		if ((PC_CAFE_DOUBLE_POINTS_CHANCE < 0) || (PC_CAFE_DOUBLE_POINTS_CHANCE > 100))
		{
			PC_CAFE_DOUBLE_POINTS_CHANCE = 1;
		}

		ACQUISITION_PC_CAFE_RETAIL_LIKE_POINTS = parser.getInt("AcquisitionPointsRetailLikePoints", 10);
		PC_CAFE_POINT_RATE = parser.getDouble("AcquisitionPointsRate", 1.0);
		PC_CAFE_RANDOM_POINT = parser.getBoolean("AcquisitionPointsRandom", false);
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
		ANNOUNCE_PK_PVP = parser.getBoolean("AnnouncePkPvP", false);
		ANNOUNCE_PK_PVP_NORMAL_MESSAGE = parser.getBoolean("AnnouncePkPvPNormalMessage", true);
		ANNOUNCE_PK_MSG = parser.getString("AnnouncePkMsg", "$killer has slaughtered $target");
		ANNOUNCE_PVP_MSG = parser.getString("AnnouncePvpMsg", "$killer has defeated $target");

		// Load PvpRewardItem config file (if exists)
		parser.LoadConfig(CUSTOM_PVP_REWARD_ITEM_CONFIG_FILE);
		REWARD_PVP_ITEM = parser.getBoolean("RewardPvpItem", false);
		REWARD_PVP_ITEM_ID = parser.getInt("RewardPvpItemId", 57);
		REWARD_PVP_ITEM_AMOUNT = parser.getInt("RewardPvpItemAmount", 1000);
		REWARD_PVP_ITEM_MESSAGE = parser.getBoolean("RewardPvpItemMessage", true);
		REWARD_PK_ITEM = parser.getBoolean("RewardPkItem", false);
		REWARD_PK_ITEM_ID = parser.getInt("RewardPkItemId", 57);
		REWARD_PK_ITEM_AMOUNT = parser.getInt("RewardPkItemAmount", 500);
		REWARD_PK_ITEM_MESSAGE = parser.getBoolean("RewardPkItemMessage", true);
		DISABLE_REWARDS_IN_INSTANCES = parser.getBoolean("DisableRewardsInInstances", true);
		DISABLE_REWARDS_IN_PVP_ZONES = parser.getBoolean("DisableRewardsInPvpZones", true);

		// Load PvpTitle config file (if exists)
		parser.LoadConfig(CUSTOM_PVP_TITLE_CONFIG_FILE);
		PVP_COLOR_SYSTEM_ENABLED = parser.getBoolean("EnablePvPColorSystem", false);
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
		ENABLE_RANDOM_MONSTER_SPAWNS = parser.getBoolean("EnableRandomMonsterSpawns", false);
		MOB_MAX_SPAWN_RANGE = parser.getInt("MaxSpawnMobRange", 150);
		MOB_MIN_SPAWN_RANGE = MOB_MAX_SPAWN_RANGE * -1;
		if (ENABLE_RANDOM_MONSTER_SPAWNS)
		{
			MOBS_LIST_NOT_RANDOM = parser.GetIntList("MobsSpawnNotRandom", ',', 18812, 18813, 18814, 22138)
				.ToImmutableSortedSet();
		}

		// Load SayuneForAll config file (if exists)
		parser.LoadConfig(CUSTOM_SAYUNE_FOR_ALL_CONFIG_FILE);
		FREE_JUMPS_FOR_ALL = parser.getBoolean("FreeJumpsForAll", false);

		// Load ScreenWelcomeMessage config file (if exists)
		parser.LoadConfig(CUSTOM_SCREEN_WELCOME_MESSAGE_CONFIG_FILE);
		WELCOME_MESSAGE_ENABLED = parser.getBoolean("ScreenWelcomeMessageEnable", false);
		WELCOME_MESSAGE_TEXT = parser.getString("ScreenWelcomeMessageText", "Welcome to our server!");
		WELCOME_MESSAGE_TIME = parser.getInt("ScreenWelcomeMessageTime", 10) * 1000;

		// Load SellBuffs config file (if exists)
		parser.LoadConfig(CUSTOM_SELL_BUFFS_CONFIG_FILE);
		SELLBUFF_ENABLED = parser.getBoolean("SellBuffEnable", false);
		SELLBUFF_MP_MULTIPLER = parser.getInt("MpCostMultipler", 1);
		SELLBUFF_PAYMENT_ID = parser.getInt("PaymentID", 57);
		SELLBUFF_MIN_PRICE = parser.getLong("MinimumPrice", 100000);
		SELLBUFF_MAX_PRICE = parser.getLong("MaximumPrice", 100000000);
		SELLBUFF_MAX_BUFFS = parser.getInt("MaxBuffs", 15);

		// Load ServerTime config file (if exists)
		parser.LoadConfig(CUSTOM_SERVER_TIME_CONFIG_FILE);
		DISPLAY_SERVER_TIME = parser.getBoolean("DisplayServerTime", false);

		// Load SchemeBuffer config file (if exists)
		parser.LoadConfig(CUSTOM_SCHEME_BUFFER_CONFIG_FILE);
		BUFFER_MAX_SCHEMES = parser.getInt("BufferMaxSchemesPerChar", 4);
		BUFFER_ITEM_ID = parser.getInt("BufferItemId", 57);
		BUFFER_STATIC_BUFF_COST = parser.getInt("BufferStaticCostPerBuff", -1);

		// Load StartingLocation config file (if exists)
		parser.LoadConfig(CUSTOM_STARTING_LOCATION_CONFIG_FILE);
		CUSTOM_STARTING_LOC = parser.getBoolean("CustomStartingLocation", false);
		CUSTOM_STARTING_LOC_X = parser.getInt("CustomStartingLocX", 50821);
		CUSTOM_STARTING_LOC_Y = parser.getInt("CustomStartingLocY", 186527);
		CUSTOM_STARTING_LOC_Z = parser.getInt("CustomStartingLocZ", -3625);

		// Load WalkerBotProtection config file (if exists)
		parser.LoadConfig(CUSTOM_WALKER_BOT_PROTECTION_CONFIG_FILE);
		L2WALKER_PROTECTION = parser.getBoolean("L2WalkerProtection", false);
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
		config.LOG_FLOODING = parser.getBoolean("FloodProtector" + configString + "LogFlooding", false);
		config.PUNISHMENT_LIMIT = parser.getInt("FloodProtector" + configString + "PunishmentLimit", 0);
		config.PUNISHMENT_TYPE = parser.getString("FloodProtector" + configString + "PunishmentType", "none");
		config.PUNISHMENT_TIME = parser.getInt("FloodProtector" + configString + "PunishmentTime", 0) * 60000;
	}

	private static ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>
		GetResurrectByPaymentList(ConfigurationParser parser, string key)
	{
		var result = ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>.Empty;
		string value = parser.getString(key);
		if (string.IsNullOrEmpty(value))
			return result;

		// Format:
		// level : times , count , restoration percent / times , count , percent;
		string[] split = value.Split(';');
		foreach (string timeData in split)
		{
			string[] timeSplit = timeData.Split(':');
			int level;
			if (timeSplit.Length != 2 || !int.TryParse(timeSplit[0], CultureInfo.InvariantCulture, out level))
			{
				LOGGER.Error(
					$"Invalid resurrect by payment item '{timeData}' in entry '{key}' in configuration file '{parser.FilePath}'");
				continue;
			}

			var resultItem = ImmutableDictionary<int, ResurrectByPaymentHolder>.Empty;
			string[] dataSplit = timeSplit[1].Split('/');
			foreach (string data in dataSplit)
			{
				string[] values = data.Split(',');
				int times;
				int count;
				double percent;
				if (values.Length != 3 || !int.TryParse(values[0], CultureInfo.InvariantCulture, out times) ||
				    !int.TryParse(values[1], CultureInfo.InvariantCulture, out count) ||
				    !double.TryParse(values[2], CultureInfo.InvariantCulture, out percent))
				{
					LOGGER.Error(
						$"Invalid resurrect by payment data '{timeData}' for level {level} in entry '{key}' in configuration file '{parser.FilePath}'");
					continue;
				}

				try
				{
					resultItem = resultItem.Add(times, new ResurrectByPaymentHolder(times, count, percent));
				}
				catch (ArgumentException)
				{
					LOGGER.Error(
						$"Duplicated key {times} in resurrect by payment data '{data}' in entry '{key}' in configuration file '{parser.FilePath}'");
				}
			}

			try
			{
				result = result.Add(level, resultItem);
			}
			catch (ArgumentException)
			{
				LOGGER.Error(
					$"Duplicated level {level} in resurrect by payment data '{timeData}' in entry '{key}' in configuration file '{parser.FilePath}'");
			}
		}

		return result;
	}

	private static ImmutableDictionary<int, int> GetSkillDurationList(ConfigurationParser parser, string key)
	{
		var result = ImmutableDictionary<int, int>.Empty;
		string value = parser.getString(key);
		if (string.IsNullOrEmpty(value))
			return result;

		string[] split = value.Split(';');
		foreach (string timeData in split)
		{
			string[] timeSplit = timeData.Split(',');
			int skillId;
			int duration;
			if (timeSplit.Length != 2 || !int.TryParse(timeSplit[0], CultureInfo.InvariantCulture, out skillId) ||
			    !int.TryParse(timeSplit[1], CultureInfo.InvariantCulture, out duration))
			{
				LOGGER.Error(
					$"Invalid skill duration item '{timeData}' in entry '{key}' in configuration file '{parser.FilePath}'");
				continue;
			}

			try
			{
				result = result.Add(skillId, duration);
			}
			catch (ArgumentException)
			{
				LOGGER.Error(
					$"Duplicated skill '{skillId}' in entry '{key}' in configuration file '{parser.FilePath}'");
			}
		}

		return result;
	}

	private static ImmutableArray<Range<int>> GetPartyXpCutoffGaps(ConfigurationParser parser, string key,
		params Range<int>[] defaultValue)
	{
		string value = parser.getString(key);
		if (string.IsNullOrEmpty(value))
			return defaultValue.ToImmutableArray();

		var result = ImmutableArray<Range<int>>.Empty.ToBuilder();
		string[] split = value.Split(';');
		foreach (string data in split)
		{
			string[] pair = data.Split(',');
			if (pair.Length != 2 || !int.TryParse(pair[0], CultureInfo.InvariantCulture, out int min) ||
			    !int.TryParse(pair[1], CultureInfo.InvariantCulture, out int max))
			{
				LOGGER.Error(
					$"Invalid format '{value}' in entry '{key}' in configuration file '{parser.FilePath}'");

				continue;
			}

			result.Add(new Range<int>(min, max));
		}

		return result.ToImmutable();
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

		var builder = ImmutableDictionary<string, Location>.Empty.ToBuilder();
		parser.GetList(key, ';', s =>
		{
			string[] item = s.Split(',');
			int x = 0;
			int y = 0;
			int z = 0;
			bool ok = int.TryParse(item[1], CultureInfo.InvariantCulture, out x) &&
			          int.TryParse(item[2], CultureInfo.InvariantCulture, out y) &&
			          int.TryParse(item[3], CultureInfo.InvariantCulture, out z);
			return ((Name: item[0], Location: new Location(x, y, z)), ok);
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