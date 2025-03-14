using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class ServerTime
    {
        public static bool DISPLAY_SERVER_TIME;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.ServerTime);

            DISPLAY_SERVER_TIME = parser.getBoolean("DisplayServerTime");
        }
    }
}