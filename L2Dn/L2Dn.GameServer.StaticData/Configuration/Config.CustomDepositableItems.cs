using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class CustomDepositableItems
    {
        public static bool CUSTOM_DEPOSITABLE_ENABLED;
        public static bool CUSTOM_DEPOSITABLE_QUEST_ITEMS;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.CustomDepositableItems);

            CUSTOM_DEPOSITABLE_ENABLED = parser.getBoolean("CustomDepositableEnabled");
            CUSTOM_DEPOSITABLE_QUEST_ITEMS = parser.getBoolean("DepositableQuestItems");
        }
    }
}