using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class HuntPass
    {
        public static bool ENABLE_HUNT_PASS;
        public static int HUNT_PASS_PREMIUM_ITEM_ID;
        public static int HUNT_PASS_PREMIUM_ITEM_COUNT;
        public static int HUNT_PASS_POINTS_FOR_STEP;
        public static int HUNT_PASS_PERIOD;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.HuntPass);

            ENABLE_HUNT_PASS = parser.getBoolean("EnabledHuntPass", true);
            HUNT_PASS_PREMIUM_ITEM_ID = parser.getInt("PremiumItemId", 91663);
            HUNT_PASS_PREMIUM_ITEM_COUNT = parser.getInt("PremiumItemCount", 3600);
            HUNT_PASS_POINTS_FOR_STEP = parser.getInt("PointsForStep", 2400);
            HUNT_PASS_PERIOD = parser.getInt("DayOfMonth", 1);
        }
    }
}