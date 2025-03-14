using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class BossAnnouncements
    {
        public static bool RAIDBOSS_SPAWN_ANNOUNCEMENTS;
        public static bool RAIDBOSS_DEFEAT_ANNOUNCEMENTS;
        public static bool RAIDBOSS_INSTANCE_ANNOUNCEMENTS;
        public static bool GRANDBOSS_SPAWN_ANNOUNCEMENTS;
        public static bool GRANDBOSS_DEFEAT_ANNOUNCEMENTS;
        public static bool GRANDBOSS_INSTANCE_ANNOUNCEMENTS;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.BossAnnouncements);

            RAIDBOSS_SPAWN_ANNOUNCEMENTS = parser.getBoolean("RaidBossSpawnAnnouncements");
            RAIDBOSS_DEFEAT_ANNOUNCEMENTS = parser.getBoolean("RaidBossDefeatAnnouncements");
            RAIDBOSS_INSTANCE_ANNOUNCEMENTS = parser.getBoolean("RaidBossInstanceAnnouncements");
            GRANDBOSS_SPAWN_ANNOUNCEMENTS = parser.getBoolean("GrandBossSpawnAnnouncements");
            GRANDBOSS_DEFEAT_ANNOUNCEMENTS = parser.getBoolean("GrandBossDefeatAnnouncements");
            GRANDBOSS_INSTANCE_ANNOUNCEMENTS = parser.getBoolean("GrandBossInstanceAnnouncements");
        }
    }
}