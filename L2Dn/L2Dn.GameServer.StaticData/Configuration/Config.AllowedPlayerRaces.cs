using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class AllowedPlayerRaces
    {
        public static bool ALLOW_HUMAN;
        public static bool ALLOW_ELF;
        public static bool ALLOW_DARKELF;
        public static bool ALLOW_ORC;
        public static bool ALLOW_DWARF;
        public static bool ALLOW_KAMAEL;
        public static bool ALLOW_DEATH_KNIGHT;
        public static bool ALLOW_SYLPH;
        public static bool ALLOW_VANGUARD;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.AllowedPlayerRaces);

            ALLOW_HUMAN = parser.getBoolean("AllowHuman", true);
            ALLOW_ELF = parser.getBoolean("AllowElf", true);
            ALLOW_DARKELF = parser.getBoolean("AllowDarkElf", true);
            ALLOW_ORC = parser.getBoolean("AllowOrc", true);
            ALLOW_DWARF = parser.getBoolean("AllowDwarf", true);
            ALLOW_KAMAEL = parser.getBoolean("AllowKamael", true);
            ALLOW_DEATH_KNIGHT = parser.getBoolean("AllowDeathKnight", true);
            ALLOW_SYLPH = parser.getBoolean("AllowSylph", true);
            ALLOW_VANGUARD = parser.getBoolean("AllowVanguard", true);
        }
    }
}