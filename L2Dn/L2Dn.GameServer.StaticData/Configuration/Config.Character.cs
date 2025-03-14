using System.Collections.Immutable;
using System.Globalization;
using L2Dn.Configuration;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Character
    {
        public static bool PLAYER_DELEVEL;
        public static int DELEVEL_MINIMUM;
        public static bool DECREASE_SKILL_LEVEL;
        public static double ALT_WEIGHT_LIMIT;
        public static int RUN_SPD_BOOST;
        public static double RESPAWN_RESTORE_CP;
        public static double RESPAWN_RESTORE_HP;
        public static double RESPAWN_RESTORE_MP;
        public static double HP_REGEN_MULTIPLIER;
        public static double MP_REGEN_MULTIPLIER;
        public static double CP_REGEN_MULTIPLIER;

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

        public static bool RESTORE_SERVITOR_ON_RECONNECT;
        public static bool RESTORE_PET_ON_RECONNECT;
        public static bool ALLOW_TRANSFORM_WITHOUT_QUEST;
        public static int FEE_DELETE_TRANSFER_SKILLS;
        public static int FEE_DELETE_SUBCLASS_SKILLS;
        public static int FEE_DELETE_DUALCLASS_SKILLS;

        // Vitality Settings
        public static bool ENABLE_VITALITY;
        public static int STARTING_VITALITY_POINTS;
        public static bool RAIDBOSS_USE_VITALITY;

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
        public static int MAX_ITEM_IN_PACKET;
        public static int WAREHOUSE_SLOTS_DWARF;
        public static int WAREHOUSE_SLOTS_NO_DWARF;
        public static int WAREHOUSE_SLOTS_CLAN;
        public static int ALT_FREIGHT_SLOTS;
        public static int ALT_FREIGHT_PRICE;
        public static long MENTOR_PENALTY_FOR_MENTEE_COMPLETE;
        public static long MENTOR_PENALTY_FOR_MENTEE_LEAVE;

        // Enchanting
        public static ImmutableSortedSet<int> ENCHANT_BLACKLIST = ImmutableSortedSet<int>.Empty;
        public static bool DISABLE_OVER_ENCHANTING;
        public static bool OVER_ENCHANT_PROTECTION;
        public static IllegalActionPunishmentType OVER_ENCHANT_PUNISHMENT;
        public static int MIN_ARMOR_ENCHANT_ANNOUNCE;
        public static int MIN_WEAPON_ENCHANT_ANNOUNCE;
        public static int MAX_ARMOR_ENCHANT_ANNOUNCE;
        public static int MAX_WEAPON_ENCHANT_ANNOUNCE;

        // Augmentation
        public static ImmutableSortedSet<int> AUGMENTATION_BLACKLIST = ImmutableSortedSet<int>.Empty;
        public static bool ALT_ALLOW_AUGMENT_PVP_ITEMS;
        public static bool ALT_ALLOW_AUGMENT_TRADE;
        public static bool ALT_ALLOW_AUGMENT_DESTROY;

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
        public static string PARTY_XP_CUTOFF_METHOD = "level";
        public static double PARTY_XP_CUTOFF_PERCENT;
        public static int PARTY_XP_CUTOFF_LEVEL;
        public static ImmutableArray<Range<int>> PARTY_XP_CUTOFF_GAPS;
        public static ImmutableArray<int> PARTY_XP_CUTOFF_GAP_PERCENTS = ImmutableArray<int>.Empty;
        public static bool DISABLE_TUTORIAL;
        public static bool STORE_RECIPE_SHOPLIST;
        public static bool STORE_UI_SETTINGS;
        public static ImmutableSortedSet<string> FORBIDDEN_NAMES = ImmutableSortedSet<string>.Empty;
        public static bool SILENCE_MODE_EXCLUDE;
        public static int PLAYER_MOVEMENT_BLOCK_TIME;
        public static int ABILITY_MAX_POINTS;
        public static long ABILITY_POINTS_RESET_ADENA;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Character);

            PLAYER_DELEVEL = parser.getBoolean("Delevel", true);
            DELEVEL_MINIMUM = parser.getInt("DelevelMinimum", 85);
            DECREASE_SKILL_LEVEL = parser.getBoolean("DecreaseSkillOnDelevel", true);
            ALT_WEIGHT_LIMIT = parser.getDouble("AltWeightLimit", 1);
            RUN_SPD_BOOST = parser.getInt("RunSpeedBoost");
            RESPAWN_RESTORE_CP = parser.getDouble("RespawnRestoreCP") / 100;
            RESPAWN_RESTORE_HP = parser.getDouble("RespawnRestoreHP", 65) / 100;
            RESPAWN_RESTORE_MP = parser.getDouble("RespawnRestoreMP") / 100;
            HP_REGEN_MULTIPLIER = parser.getDouble("HpRegenMultiplier", 100) / 100;
            MP_REGEN_MULTIPLIER = parser.getDouble("MpRegenMultiplier", 100) / 100;
            CP_REGEN_MULTIPLIER = parser.getDouble("CpRegenMultiplier", 100) / 100;

            RESURRECT_BY_PAYMENT_ENABLED = parser.getBoolean("EnabledResurrectByPay", true);
            if (RESURRECT_BY_PAYMENT_ENABLED)
            {
                RESURRECT_BY_PAYMENT_MAX_FREE_TIMES = parser.getInt("MaxFreeResurrectionsByDay");
                RESURRECT_BY_PAYMENT_FIRST_RESURRECT_ITEM = parser.getInt("FirstResurrectItemID");
                RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES = GetResurrectByPaymentList(parser, "FirstResurrectList");
                RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES = GetResurrectByPaymentList(parser, "SecondResurrectList");
                RESURRECT_BY_PAYMENT_SECOND_RESURRECT_ITEM = parser.getInt("SecondResurrectItemID");
            }

            ENABLE_MODIFY_SKILL_DURATION = parser.getBoolean("EnableModifySkillDuration");
            if (ENABLE_MODIFY_SKILL_DURATION)
            {
                SKILL_DURATION_LIST = GetSkillDurationList(parser, "SkillDurationList");
            }

            ENABLE_MODIFY_SKILL_REUSE = parser.getBoolean("EnableModifySkillReuse");
            if (ENABLE_MODIFY_SKILL_REUSE)
            {
                SKILL_REUSE_LIST = GetSkillDurationList(parser, "SkillReuseList");
            }

            AUTO_LEARN_SKILLS = parser.getBoolean("AutoLearnSkills");
            AUTO_LEARN_SKILLS_WITHOUT_ITEMS = parser.getBoolean("AutoLearnSkillsWithoutItems");
            AUTO_LEARN_FS_SKILLS = parser.getBoolean("AutoLearnForgottenScrollSkills");
            AUTO_LOOT_HERBS = parser.getBoolean("AutoLootHerbs");
            BUFFS_MAX_AMOUNT = parser.getByte("MaxBuffAmount", 20);
            TRIGGERED_BUFFS_MAX_AMOUNT = parser.getByte("MaxTriggeredBuffAmount", 12);
            DANCES_MAX_AMOUNT = parser.getByte("MaxDanceAmount", 12);
            DANCE_CANCEL_BUFF = parser.getBoolean("DanceCancelBuff");
            DANCE_CONSUME_ADDITIONAL_MP = parser.getBoolean("DanceConsumeAdditionalMP", true);
            ALT_STORE_DANCES = parser.getBoolean("AltStoreDances");
            AUTO_LEARN_DIVINE_INSPIRATION = parser.getBoolean("AutoLearnDivineInspiration");

            ALT_GAME_CANCEL_BOW = string.Equals(parser.getString("AltGameCancelByHit", "Cast"), "bow",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(parser.getString("AltGameCancelByHit", "Cast"), "all",
                    StringComparison.OrdinalIgnoreCase);

            ALT_GAME_CANCEL_CAST = string.Equals(parser.getString("AltGameCancelByHit", "Cast"), "cast",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(parser.getString("AltGameCancelByHit", "Cast"), "all",
                    StringComparison.OrdinalIgnoreCase);

            ALT_GAME_MAGICFAILURES = parser.getBoolean("MagicFailures", true);
            ALT_GAME_STUN_BREAK = parser.getBoolean("BreakStun");
            PLAYER_FAKEDEATH_UP_PROTECTION = parser.getInt("PlayerFakeDeathUpProtection");
            STORE_SKILL_COOLTIME = parser.getBoolean("StoreSkillCooltime", true);
            SUBCLASS_STORE_SKILL_COOLTIME = parser.getBoolean("SubclassStoreSkillCooltime");
            SUMMON_STORE_SKILL_COOLTIME = parser.getBoolean("SummonStoreSkillCooltime", true);
            EFFECT_TICK_RATIO = parser.getLong("EffectTickRatio", 666);
            FAKE_DEATH_UNTARGET = parser.getBoolean("FakeDeathUntarget", true);
            FAKE_DEATH_DAMAGE_STAND = parser.getBoolean("FakeDeathDamageStand");
            VAMPIRIC_ATTACK_WORKS_WITH_SKILLS = parser.getBoolean("VampiricAttackWorkWithSkills", true);
            MP_VAMPIRIC_ATTACK_WORKS_WITH_MELEE = parser.getBoolean("MpVampiricAttackWorkWithMelee");
            CALCULATE_MAGIC_SUCCESS_BY_SKILL_MAGIC_LEVEL =
                parser.getBoolean("CalculateMagicSuccessBySkillMagicLevel");

            BLOW_RATE_CHANCE_LIMIT = parser.getInt("BlowRateChanceLimit", 100);
            ITEM_EQUIP_ACTIVE_SKILL_REUSE = parser.getInt("ItemEquipActiveSkillReuse", 300000);
            ARMOR_SET_EQUIP_ACTIVE_SKILL_REUSE = parser.getInt("ArmorSetEquipActiveSkillReuse", 60000);
            PLAYER_REFLECT_PERCENT_LIMIT = parser.getDouble("PlayerReflectPercentLimit", 100);
            NON_PLAYER_REFLECT_PERCENT_LIMIT = parser.getDouble("NonPlayerReflectPercentLimit", 100);
            LIFE_CRYSTAL_NEEDED = parser.getBoolean("LifeCrystalNeeded", true);
            DIVINE_SP_BOOK_NEEDED = parser.getBoolean("DivineInspirationSpBookNeeded", true);
            ALT_GAME_SUBCLASS_WITHOUT_QUESTS = parser.getBoolean("AltSubClassWithoutQuests");
            ALT_GAME_SUBCLASS_EVERYWHERE = parser.getBoolean("AltSubclassEverywhere");
            RESTORE_SERVITOR_ON_RECONNECT = parser.getBoolean("RestoreServitorOnReconnect", true);
            RESTORE_PET_ON_RECONNECT = parser.getBoolean("RestorePetOnReconnect", true);
            ALLOW_TRANSFORM_WITHOUT_QUEST = parser.getBoolean("AltTransformationWithoutQuest");
            FEE_DELETE_TRANSFER_SKILLS = parser.getInt("FeeDeleteTransferSkills", 10000000);
            FEE_DELETE_SUBCLASS_SKILLS = parser.getInt("FeeDeleteSubClassSkills", 10000000);
            FEE_DELETE_DUALCLASS_SKILLS = parser.getInt("FeeDeleteDualClassSkills", 20000000);

            // Vitality Settings
            ENABLE_VITALITY = parser.getBoolean("EnableVitality", true);
            STARTING_VITALITY_POINTS = parser.getInt("StartingVitalityPoints", 140000);
            RAIDBOSS_USE_VITALITY = parser.getBoolean("RaidbossUseVitality", true);

            MAX_BONUS_EXP = parser.getDouble("MaxExpBonus");
            MAX_BONUS_SP = parser.getDouble("MaxSpBonus");
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
            MENTOR_PENALTY_FOR_MENTEE_COMPLETE =
                parser.getInt("MentorPenaltyForMenteeComplete", 1) * 24 * 60 * 60 * 1000;

            MENTOR_PENALTY_FOR_MENTEE_COMPLETE = parser.getInt("MentorPenaltyForMenteeLeave", 2) * 24 * 60 * 60 * 1000;

            // Enchanting
            ENCHANT_BLACKLIST = parser.GetIntList("EnchantBlackList", ',', 7816, 7817, 7818, 7819, 7820, 7821, 7822,
                7823,
                7824, 7825, 7826, 7827, 7828, 7829, 7830, 7831, 13293, 13294, 13296).ToImmutableSortedSet();

            DISABLE_OVER_ENCHANTING = parser.getBoolean("DisableOverEnchanting", true);
            OVER_ENCHANT_PROTECTION = parser.getBoolean("OverEnchantProtection", true);
            OVER_ENCHANT_PUNISHMENT = parser.GetEnum("OverEnchantPunishment", IllegalActionPunishmentType.JAIL);
            MIN_ARMOR_ENCHANT_ANNOUNCE = parser.getInt("MinimumArmorEnchantAnnounce", 6);
            MIN_WEAPON_ENCHANT_ANNOUNCE = parser.getInt("MinimumWeaponEnchantAnnounce", 7);
            MAX_ARMOR_ENCHANT_ANNOUNCE = parser.getInt("MaximumArmorEnchantAnnounce", 30);
            MAX_WEAPON_ENCHANT_ANNOUNCE = parser.getInt("MaximumWeaponEnchantAnnounce", 30);

            // Augmentation
            AUGMENTATION_BLACKLIST = parser.GetIntList("AugmentationBlackList").ToImmutableSortedSet();
            ALT_ALLOW_AUGMENT_PVP_ITEMS = parser.getBoolean("AltAllowAugmentPvPItems");
            ALT_ALLOW_AUGMENT_TRADE = parser.getBoolean("AltAllowAugmentTrade");
            ALT_ALLOW_AUGMENT_DESTROY = parser.getBoolean("AltAllowAugmentDestroy", true);

            ALT_GAME_KARMA_PLAYER_CAN_BE_KILLED_IN_PEACEZONE =
                parser.getBoolean("AltKarmaPlayerCanBeKilledInPeaceZone");

            ALT_GAME_KARMA_PLAYER_CAN_SHOP = parser.getBoolean("AltKarmaPlayerCanShop", true);
            ALT_GAME_KARMA_PLAYER_CAN_TELEPORT = parser.getBoolean("AltKarmaPlayerCanTeleport", true);
            ALT_GAME_KARMA_PLAYER_CAN_USE_GK = parser.getBoolean("AltKarmaPlayerCanUseGK");
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
            ALT_GAME_CREATION = parser.getBoolean("AltGameCreation");
            ALT_GAME_CREATION_SPEED = parser.getDouble("AltGameCreationSpeed", 1);
            ALT_GAME_CREATION_XP_RATE = parser.getDouble("AltGameCreationXpRate", 1);
            ALT_GAME_CREATION_SP_RATE = parser.getDouble("AltGameCreationSpRate", 1);
            ALT_GAME_CREATION_RARE_XPSP_RATE = parser.getDouble("AltGameCreationRareXpSpRate", 2);
            ALT_CLAN_LEADER_INSTANT_ACTIVATION = parser.getBoolean("AltClanLeaderInstantActivation");
            ALT_CLAN_JOIN_MINS = parser.getInt("MinutesBeforeJoinAClan", 1);
            ALT_CLAN_CREATE_DAYS = parser.getInt("DaysBeforeCreateAClan", 10);
            ALT_CLAN_DISSOLVE_DAYS = parser.getInt("DaysToPassToDissolveAClan", 7);
            ALT_ALLY_JOIN_DAYS_WHEN_LEAVED = parser.getInt("DaysBeforeJoinAllyWhenLeaved", 1);
            ALT_ALLY_JOIN_DAYS_WHEN_DISMISSED = parser.getInt("DaysBeforeJoinAllyWhenDismissed", 1);
            ALT_ACCEPT_CLAN_DAYS_WHEN_DISMISSED = parser.getInt("DaysBeforeAcceptNewClanWhenDismissed", 1);
            ALT_CREATE_ALLY_DAYS_WHEN_DISSOLVED = parser.getInt("DaysBeforeCreateNewAllyWhenDissolved", 1);
            ALT_MAX_NUM_OF_CLANS_IN_ALLY = parser.getInt("AltMaxNumOfClansInAlly", 3);
            ALT_CLAN_MEMBERS_FOR_WAR = parser.getInt("AltClanMembersForWar", 15);
            ALT_MEMBERS_CAN_WITHDRAW_FROM_CLANWH = parser.getBoolean("AltMembersCanWithdrawFromClanWH");
            ALT_CLAN_MEMBERS_TIME_FOR_BONUS =
                parser.GetTimeSpan("AltClanMembersTimeForBonus", TimeSpan.FromMinutes(30));

            REMOVE_CASTLE_CIRCLETS = parser.getBoolean("RemoveCastleCirclets", true);
            ALT_PARTY_MAX_MEMBERS = parser.getInt("AltPartyMaxMembers", 7);
            ALT_PARTY_RANGE = parser.getInt("AltPartyRange", 1500);
            ALT_LEAVE_PARTY_LEADER = parser.getBoolean("AltLeavePartyLeader");
            ALT_COMMAND_CHANNEL_FRIENDS = parser.getBoolean("AltCommandChannelFriends");
            INITIAL_EQUIPMENT_EVENT = parser.getBoolean("InitialEquipmentEvent");
            STARTING_ADENA = parser.getLong("StartingAdena");
            STARTING_LEVEL = parser.getInt("StartingLevel", 1);
            STARTING_SP = parser.getInt("StartingSP");
            MAX_ADENA = parser.getLong("MaxAdena", 99900000000L);
            if (MAX_ADENA < 0)
                MAX_ADENA = long.MaxValue;

            AUTO_LOOT = parser.getBoolean("AutoLoot");
            AUTO_LOOT_RAIDS = parser.getBoolean("AutoLootRaids");
            AUTO_LOOT_SLOT_LIMIT = parser.getBoolean("AutoLootSlotLimit");
            LOOT_RAIDS_PRIVILEGE_INTERVAL = parser.getInt("RaidLootRightsInterval", 900) * 1000;
            LOOT_RAIDS_PRIVILEGE_CC_SIZE = parser.getInt("RaidLootRightsCCSize", 45);
            AUTO_LOOT_ITEM_IDS = parser.GetIntList("AutoLootItemIds").ToImmutableSortedSet();
            ENABLE_KEYBOARD_MOVEMENT = parser.getBoolean("KeyboardMovement", true);
            UNSTUCK_INTERVAL = parser.getInt("UnstuckInterval", 300);
            TELEPORT_WATCHDOG_TIMEOUT = parser.getInt("TeleportWatchdogTimeout");
            PLAYER_SPAWN_PROTECTION = parser.getInt("PlayerSpawnProtection");
            PLAYER_TELEPORT_PROTECTION = parser.getInt("PlayerTeleportProtection");
            RANDOM_RESPAWN_IN_TOWN_ENABLED = parser.getBoolean("RandomRespawnInTownEnabled", true);
            OFFSET_ON_TELEPORT_ENABLED = parser.getBoolean("OffsetOnTeleportEnabled", true);
            MAX_OFFSET_ON_TELEPORT = parser.getInt("MaxOffsetOnTeleport", 50);
            TELEPORT_WHILE_SIEGE_IN_PROGRESS = parser.getBoolean("TeleportWhileSiegeInProgress", true);
            TELEPORT_WHILE_PLAYER_IN_COMBAT = parser.getBoolean("TeleportWhilePlayerInCombat");
            PETITIONING_ALLOWED = parser.getBoolean("PetitioningAllowed", true);
            MAX_PETITIONS_PER_PLAYER = parser.getInt("MaxPetitionsPerPlayer", 5);
            MAX_PETITIONS_PENDING = parser.getInt("MaxPetitionsPending", 25);
            MAX_FREE_TELEPORT_LEVEL = parser.getInt("MaxFreeTeleportLevel", 99);
            MAX_NEWBIE_BUFF_LEVEL = parser.getInt("MaxNewbieBuffLevel");
            DELETE_DAYS = parser.getInt("DeleteCharAfterDays", 1);
            DISCONNECT_AFTER_DEATH = parser.getBoolean("DisconnectAfterDeath", true);
            PARTY_XP_CUTOFF_METHOD = parser.getString("PartyXpCutoffMethod", "level").ToLower();
            PARTY_XP_CUTOFF_PERCENT = parser.getDouble("PartyXpCutoffPercent", 3);
            PARTY_XP_CUTOFF_LEVEL = parser.getInt("PartyXpCutoffLevel", 20);
            PARTY_XP_CUTOFF_GAPS = GetPartyXpCutoffGaps(parser, "PartyXpCutoffGaps",
                new(0, 9), new(10, 14), new(15, 99));

            PARTY_XP_CUTOFF_GAP_PERCENTS = parser.GetIntList("PartyXpCutoffGapPercent", ';', 100, 30, 0);
            DISABLE_TUTORIAL = parser.getBoolean("DisableTutorial");
            STORE_RECIPE_SHOPLIST = parser.getBoolean("StoreRecipeShopList");
            STORE_UI_SETTINGS = parser.getBoolean("StoreCharUiSettings", true);
            FORBIDDEN_NAMES = parser.GetStringList("ForbiddenNames").
                ToImmutableSortedSet(StringComparer.CurrentCultureIgnoreCase);

            SILENCE_MODE_EXCLUDE = parser.getBoolean("SilenceModeExclude");
            PLAYER_MOVEMENT_BLOCK_TIME = parser.getInt("NpcTalkBlockingTime") * 1000;
            ABILITY_MAX_POINTS = parser.getInt("AbilityMaxPoints", 16);
            ABILITY_POINTS_RESET_ADENA = parser.getLong("AbilityPointsResetAdena", 10_000_000);
        }

        private static ImmutableDictionary<int, TimeSpan> GetSkillDurationList(ConfigurationParser parser, string key)
        {
            var result = ImmutableDictionary<int, TimeSpan>.Empty;
            string value = parser.getString(key);
            if (string.IsNullOrWhiteSpace(value))
                return result;

            string[] split = value.Split(';');
            foreach (string timeData in split)
            {
                string[] timeSplit = timeData.Split(',');
                if (timeSplit.Length != 2 ||
                    !int.TryParse(timeSplit[0], CultureInfo.InvariantCulture, out int skillId) ||
                    !int.TryParse(timeSplit[1], CultureInfo.InvariantCulture, out int duration))
                {
                    _logger.Error(
                        $"Invalid skill duration item '{timeData}' in entry '{key}' in configuration file '{parser.FilePath}'");

                    continue;
                }

                try
                {
                    result = result.Add(skillId, TimeSpan.FromMilliseconds(duration));
                }
                catch (ArgumentException)
                {
                    _logger.Error(
                        $"Duplicated skill '{skillId}' in entry '{key}' in configuration file '{parser.FilePath}'");
                }
            }

            return result;
        }

        private static ImmutableArray<Range<int>> GetPartyXpCutoffGaps(ConfigurationParser parser, string key,
            params ReadOnlySpan<Range<int>> defaultValue)
        {
            string value = parser.getString(key);
            if (string.IsNullOrWhiteSpace(value))
                return [..defaultValue];

            var result = ImmutableArray<Range<int>>.Empty.ToBuilder();
            string[] split = value.Split(';');
            foreach (string data in split)
            {
                string[] pair = data.Split(',');
                if (pair.Length != 2 || !int.TryParse(pair[0], CultureInfo.InvariantCulture, out int min) ||
                    !int.TryParse(pair[1], CultureInfo.InvariantCulture, out int max))
                {
                    _logger.Error(
                        $"Invalid format '{value}' in entry '{key}' in configuration file '{parser.FilePath}'");

                    continue;
                }

                result.Add(new Range<int>(min, max));
            }

            return result.ToImmutable();
        }

        private static ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>
            GetResurrectByPaymentList(ConfigurationParser parser, string key)
        {
            var result = ImmutableDictionary<int, ImmutableDictionary<int, ResurrectByPaymentHolder>>.Empty;
            string value = parser.getString(key);
            if (string.IsNullOrWhiteSpace(value))
                return result;

            value = value.Trim();
            if (value.EndsWith(';'))
                value = value[..^1];

            // Format:
            // level : times , count , restoration percent / times , count , percent;
            string[] split = value.Split(';');
            foreach (string timeData in split)
            {
                string[] timeSplit = timeData.Split(':');
                if (timeSplit.Length != 2 || !int.TryParse(timeSplit[0], CultureInfo.InvariantCulture, out int level))
                {
                    _logger.Error(
                        $"Invalid resurrect by payment item '{timeData}' in entry '{key}' in configuration file '{parser.FilePath}'");

                    continue;
                }

                var resultItem = ImmutableDictionary<int, ResurrectByPaymentHolder>.Empty;
                string[] dataSplit = timeSplit[1].Split('/');
                foreach (string data in dataSplit)
                {
                    string[] values = data.Split(',');
                    if (values.Length != 3 || !int.TryParse(values[0], CultureInfo.InvariantCulture, out int times) ||
                        !int.TryParse(values[1], CultureInfo.InvariantCulture, out int count) ||
                        !double.TryParse(values[2], CultureInfo.InvariantCulture, out double percent))
                    {
                        _logger.Error(
                            $"Invalid resurrect by payment data '{timeData}' for level {level} in entry '{key}' in configuration file '{parser.FilePath}'");

                        continue;
                    }

                    try
                    {
                        resultItem = resultItem.Add(times, new ResurrectByPaymentHolder(times, count, percent));
                    }
                    catch (ArgumentException)
                    {
                        _logger.Error(
                            $"Duplicated key {times} in resurrect by payment data '{data}' in entry '{key}' in configuration file '{parser.FilePath}'");
                    }
                }

                try
                {
                    result = result.Add(level, resultItem);
                }
                catch (ArgumentException)
                {
                    _logger.Error(
                        $"Duplicated level {level} in resurrect by payment data '{timeData}' in entry '{key}' in configuration file '{parser.FilePath}'");
                }
            }

            return result;
        }
    }
}