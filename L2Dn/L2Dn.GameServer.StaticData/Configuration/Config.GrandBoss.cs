using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class GrandBoss
    {
        // Antharas
        public static int ANTHARAS_WAIT_TIME;
        public static int ANTHARAS_SPAWN_INTERVAL;
        public static int ANTHARAS_SPAWN_RANDOM;

        // Baium
        public static int BAIUM_SPAWN_INTERVAL;

        // Core
        public static int CORE_SPAWN_INTERVAL;
        public static int CORE_SPAWN_RANDOM;

        // Offen
        public static int ORFEN_SPAWN_INTERVAL;
        public static int ORFEN_SPAWN_RANDOM;

        // Queen Ant
        public static int QUEEN_ANT_SPAWN_INTERVAL;
        public static int QUEEN_ANT_SPAWN_RANDOM;

        // Zaken
        public static int ZAKEN_SPAWN_INTERVAL;
        public static int ZAKEN_SPAWN_RANDOM;

        // Balok
        public static TimeOnly BALOK_TIME;
        public static int BALOK_POINTS_PER_MONSTER;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.GrandBoss);

            ANTHARAS_WAIT_TIME = parser.getInt("AntharasWaitTime", 30);
            ANTHARAS_SPAWN_INTERVAL = parser.getInt("IntervalOfAntharasSpawn", 264);
            ANTHARAS_SPAWN_RANDOM = parser.getInt("RandomOfAntharasSpawn", 72);
            BAIUM_SPAWN_INTERVAL = parser.getInt("IntervalOfBaiumSpawn", 168);
            CORE_SPAWN_INTERVAL = parser.getInt("IntervalOfCoreSpawn", 60);
            CORE_SPAWN_RANDOM = parser.getInt("RandomOfCoreSpawn", 24);
            ORFEN_SPAWN_INTERVAL = parser.getInt("IntervalOfOrfenSpawn", 48);
            ORFEN_SPAWN_RANDOM = parser.getInt("RandomOfOrfenSpawn", 20);
            QUEEN_ANT_SPAWN_INTERVAL = parser.getInt("IntervalOfQueenAntSpawn", 36);
            QUEEN_ANT_SPAWN_RANDOM = parser.getInt("RandomOfQueenAntSpawn", 17);
            ZAKEN_SPAWN_INTERVAL = parser.getInt("IntervalOfZakenSpawn", 168);
            ZAKEN_SPAWN_RANDOM = parser.getInt("RandomOfZakenSpawn", 48);
            BALOK_TIME = parser.GetTime("BalokTime", TimeOnly.FromTimeSpan(new TimeSpan(20, 30, 0)));
            BALOK_POINTS_PER_MONSTER = parser.getInt("BalokPointsPerMonster", 10);
        }
    }
}