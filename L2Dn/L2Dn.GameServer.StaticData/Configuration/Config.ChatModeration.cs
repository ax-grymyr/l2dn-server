using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class ChatModeration
    {
        public static bool CHAT_ADMIN;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.ChatModeration);

            CHAT_ADMIN = parser.getBoolean("ChatAdmin", true);
        }
    }
}