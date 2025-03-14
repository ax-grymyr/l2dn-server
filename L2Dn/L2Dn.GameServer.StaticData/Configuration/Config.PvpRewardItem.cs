using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class PvpRewardItem
    {
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

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.PvpRewardItem);

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
        }
    }
}