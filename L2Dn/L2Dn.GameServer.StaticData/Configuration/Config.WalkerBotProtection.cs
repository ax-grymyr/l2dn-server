using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class WalkerBotProtection
    {
        public static bool L2WALKER_PROTECTION;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.WalkerBotProtection);

            L2WALKER_PROTECTION = parser.getBoolean("L2WalkerProtection");
        }
    }
}