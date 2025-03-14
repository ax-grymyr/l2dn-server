using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class GameAssistant
    {
        public static bool GAME_ASSISTANT_ENABLED;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.GameAssistant);

            GAME_ASSISTANT_ENABLED = parser.getBoolean("GameAssistantEnabled");
        }
    }
}