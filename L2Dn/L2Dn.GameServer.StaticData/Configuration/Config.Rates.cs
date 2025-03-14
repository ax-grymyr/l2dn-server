using System.Collections.Immutable;
using System.Globalization;
using L2Dn.Configuration;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Rates
    {
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

        // Vitality Settings
        public static float RATE_VITALITY_EXP_MULTIPLIER;
        public static float RATE_LIMITED_SAYHA_GRACE_EXP_MULTIPLIER;
        public static int VITALITY_MAX_ITEMS_ALLOWED;
        public static float RATE_VITALITY_LOST;
        public static float RATE_VITALITY_GAIN;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Rates);

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
                VITALITY_MAX_ITEMS_ALLOWED = int.MaxValue;

            RATE_VITALITY_LOST = parser.getFloat("RateVitalityLost", 1);
            RATE_VITALITY_GAIN = parser.getFloat("RateVitalityGain", 1);
            RATE_KARMA_LOST = parser.getFloat("RateKarmaLost", -1);
            if (RATE_KARMA_LOST == -1)
                RATE_KARMA_LOST = RATE_XP;

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
        }
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
}