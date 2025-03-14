using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class TrainingCamp
    {
        public static bool TRAINING_CAMP_ENABLE;
        public static bool TRAINING_CAMP_PREMIUM_ONLY;
        public static int TRAINING_CAMP_MAX_DURATION;
        public static int TRAINING_CAMP_MIN_LEVEL;
        public static int TRAINING_CAMP_MAX_LEVEL;
        public static double TRAINING_CAMP_EXP_MULTIPLIER;
        public static double TRAINING_CAMP_SP_MULTIPLIER;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.TrainingCamp);

            TRAINING_CAMP_ENABLE = parser.getBoolean("TrainingCampEnable");
            TRAINING_CAMP_PREMIUM_ONLY = parser.getBoolean("TrainingCampPremiumOnly");
            TRAINING_CAMP_MAX_DURATION = parser.getInt("TrainingCampDuration", 18000);
            TRAINING_CAMP_MIN_LEVEL = parser.getInt("TrainingCampMinLevel", 18);
            TRAINING_CAMP_MAX_LEVEL = parser.getInt("TrainingCampMaxLevel", 127);
            TRAINING_CAMP_EXP_MULTIPLIER = parser.getDouble("TrainingCampExpMultiplier", 1.0);
            TRAINING_CAMP_SP_MULTIPLIER = parser.getDouble("TrainingCampSpMultiplier", 1.0);
        }
    }
}