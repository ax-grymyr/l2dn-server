using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class AchievementBox
    {
        public static bool ENABLE_ACHIEVEMENT_BOX;
        public static int ACHIEVEMENT_BOX_POINTS_FOR_REWARD;
        public static bool ENABLE_ACHIEVEMENT_PVP;
        public static int ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.AchievementBox);

            ENABLE_ACHIEVEMENT_BOX = parser.getBoolean("EnabledAchievementBox", true);
            ACHIEVEMENT_BOX_POINTS_FOR_REWARD = parser.getInt("PointsForReward", 1000);
            ENABLE_ACHIEVEMENT_PVP = parser.getBoolean("EnabledAchievementPvP", true);
            ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD = parser.getInt("PointsForPvpReward", 5);
        }
    }
}