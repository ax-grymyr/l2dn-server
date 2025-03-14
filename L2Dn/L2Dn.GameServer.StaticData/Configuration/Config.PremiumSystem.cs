using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class PremiumSystem
    {
        public static bool PREMIUM_SYSTEM_ENABLED;
        public static float PREMIUM_RATE_XP;
        public static float PREMIUM_RATE_SP;
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

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.PremiumSystem);

            PREMIUM_SYSTEM_ENABLED = parser.getBoolean("EnablePremiumSystem");
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

            PC_CAFE_ENABLED = parser.getBoolean("PcCafeEnabled");
            PC_CAFE_ONLY_PREMIUM = parser.getBoolean("PcCafeOnlyPremium");
            PC_CAFE_ONLY_VIP = parser.getBoolean("PcCafeOnlyVip");
            PC_CAFE_RETAIL_LIKE = parser.getBoolean("PcCafeRetailLike", true);
            PC_CAFE_REWARD_TIME = parser.getInt("PcCafeRewardTime", 300000);
            PC_CAFE_MAX_POINTS = parser.getInt("MaxPcCafePoints", 200000);
            if (PC_CAFE_MAX_POINTS < 0)
                PC_CAFE_MAX_POINTS = 0;

            PC_CAFE_ENABLE_DOUBLE_POINTS = parser.getBoolean("DoublingAcquisitionPoints");
            PC_CAFE_DOUBLE_POINTS_CHANCE = parser.getInt("DoublingAcquisitionPointsChance", 1);
            if (PC_CAFE_DOUBLE_POINTS_CHANCE < 0 || PC_CAFE_DOUBLE_POINTS_CHANCE > 100)
                PC_CAFE_DOUBLE_POINTS_CHANCE = 1;

            ACQUISITION_PC_CAFE_RETAIL_LIKE_POINTS = parser.getInt("AcquisitionPointsRetailLikePoints", 10);
            PC_CAFE_POINT_RATE = parser.getDouble("AcquisitionPointsRate", 1.0);
            PC_CAFE_RANDOM_POINT = parser.getBoolean("AcquisitionPointsRandom");
            if (PC_CAFE_POINT_RATE < 0)
                PC_CAFE_POINT_RATE = 1;

            PC_CAFE_REWARD_LOW_EXP_KILLS = parser.getBoolean("RewardLowExpKills", true);
            PC_CAFE_LOW_EXP_KILLS_CHANCE = parser.getInt("RewardLowExpKillsChance", 50);
            if (PC_CAFE_LOW_EXP_KILLS_CHANCE < 0)
                PC_CAFE_LOW_EXP_KILLS_CHANCE = 0;

            if (PC_CAFE_LOW_EXP_KILLS_CHANCE > 100)
                PC_CAFE_LOW_EXP_KILLS_CHANCE = 100;
        }
    }
}