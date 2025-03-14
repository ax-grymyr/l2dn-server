using System.Collections.Immutable;
using L2Dn.Configuration;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class General
    {
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
        public static bool ALLOW_MANOR;
        public static bool ALLOW_BOAT;
        public static int BOAT_BROADCAST_RADIUS;
        public static bool ALLOW_CURSED_WEAPONS;
        public static bool SERVER_NEWS;
        public static bool ENABLE_COMMUNITY_BOARD;
        public static string BBS_DEFAULT = "_bbshome";
        public static bool USE_SAY_FILTER;
        public static string CHAT_FILTER_CHARS = "^_^";
        public static ImmutableSortedSet<ChatType> BAN_CHAT_CHANNELS = ImmutableSortedSet<ChatType>.Empty;
        public static int WORLD_CHAT_MIN_LEVEL;
        public static int WORLD_CHAT_POINTS_PER_DAY;
        public static TimeSpan WORLD_CHAT_INTERVAL;
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

        public static string SUBJUGATION_TOPIC_BODY =
            "Reward for being in the top of the best players in clearing the lands of Aden";

        public static string SUBJUGATION_TOPIC_HEADER = "Purge reward";

        public static int SHARING_LOCATION_COST;
        public static int TELEPORT_SHARE_LOCATION_COST;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.General);

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
            ALT_DEV_EXCLUDED_PACKETS = parser.GetStringList("ExcludedPacketList").
                ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);

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
        }
    }
}