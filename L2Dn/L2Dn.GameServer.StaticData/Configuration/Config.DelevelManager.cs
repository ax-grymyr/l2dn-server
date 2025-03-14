using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class DelevelManager
    {
        public static bool DELEVEL_MANAGER_ENABLED;
        public static int DELEVEL_MANAGER_NPCID;
        public static int DELEVEL_MANAGER_ITEMID;
        public static int DELEVEL_MANAGER_ITEMCOUNT;
        public static int DELEVEL_MANAGER_MINIMUM_DELEVEL;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.DelevelManager);

            DELEVEL_MANAGER_ENABLED = parser.getBoolean("Enabled");
            DELEVEL_MANAGER_NPCID = parser.getInt("NpcId", 1002000);
            DELEVEL_MANAGER_ITEMID = parser.getInt("RequiredItemId", 4356);
            DELEVEL_MANAGER_ITEMCOUNT = parser.getInt("RequiredItemCount", 2);
            DELEVEL_MANAGER_MINIMUM_DELEVEL = parser.getInt("MimimumDelevel", 20);
        }
    }
}