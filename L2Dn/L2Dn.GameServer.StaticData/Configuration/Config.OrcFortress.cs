using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class OrcFortress
    {
        public static bool ORC_FORTRESS_ENABLE;
        public static TimeOnly ORC_FORTRESS_TIME;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.OrcFortress);

            ORC_FORTRESS_TIME = parser.GetTime("OrcFortressTime", TimeOnly.FromTimeSpan(new TimeSpan(20, 0, 0)));
            ORC_FORTRESS_ENABLE = parser.getBoolean("OrcFortressEnable", true);
        }
    }
}