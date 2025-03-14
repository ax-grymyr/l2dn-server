using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class PasswordChange
    {
        public static bool ALLOW_CHANGE_PASSWORD;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.PasswordChange);

            ALLOW_CHANGE_PASSWORD = parser.getBoolean("AllowChangePassword");
        }
    }
}