using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class OnlineInfo
    {
        public static bool ENABLE_ONLINE_COMMAND;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.OnlineInfo);

            ENABLE_ONLINE_COMMAND = parser.getBoolean("EnableOnlineCommand");
        }
    }
}