using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class CustomMailManager
    {
        public static bool CUSTOM_MAIL_MANAGER_ENABLED;
        public static int CUSTOM_MAIL_MANAGER_DELAY;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.CustomMailManager);

            CUSTOM_MAIL_MANAGER_ENABLED = parser.getBoolean("CustomMailManagerEnabled");
            CUSTOM_MAIL_MANAGER_DELAY = parser.getInt("DatabaseQueryDelay", 30) * 1000;
        }
    }
}