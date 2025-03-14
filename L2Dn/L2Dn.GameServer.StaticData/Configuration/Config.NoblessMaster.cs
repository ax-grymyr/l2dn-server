using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class NoblessMaster
    {
        public static bool NOBLESS_MASTER_ENABLED;
        public static int NOBLESS_MASTER_NPCID;
        public static int NOBLESS_MASTER_LEVEL_REQUIREMENT;
        public static bool NOBLESS_MASTER_REWARD_TIARA;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.NoblessMaster);

            NOBLESS_MASTER_ENABLED = parser.getBoolean("Enabled");
            NOBLESS_MASTER_NPCID = parser.getInt("NpcId", 1003000);
            NOBLESS_MASTER_LEVEL_REQUIREMENT = parser.getInt("LevelRequirement", 80);
            NOBLESS_MASTER_REWARD_TIARA = parser.getBoolean("RewardTiara");
        }
    }
}