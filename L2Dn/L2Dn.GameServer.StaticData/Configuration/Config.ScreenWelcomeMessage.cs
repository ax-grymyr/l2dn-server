using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class ScreenWelcomeMessage
    {
        public static bool WELCOME_MESSAGE_ENABLED;
        public static string WELCOME_MESSAGE_TEXT = "Welcome to our server!";
        public static int WELCOME_MESSAGE_TIME;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.ScreenWelcomeMessage);

            WELCOME_MESSAGE_ENABLED = parser.getBoolean("ScreenWelcomeMessageEnable");
            WELCOME_MESSAGE_TEXT = parser.getString("ScreenWelcomeMessageText", "Welcome to our server!");
            WELCOME_MESSAGE_TIME = parser.getInt("ScreenWelcomeMessageTime", 10) * 1000;
        }
    }
}