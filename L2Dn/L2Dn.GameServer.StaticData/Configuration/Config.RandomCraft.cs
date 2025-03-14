using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class RandomCraft
    {
        public static bool ENABLE_RANDOM_CRAFT;
        public static int RANDOM_CRAFT_REFRESH_FEE;
        public static int RANDOM_CRAFT_CREATE_FEE;
        public static bool DROP_RANDOM_CRAFT_MATERIALS;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.RandomCraft);

            ENABLE_RANDOM_CRAFT = parser.getBoolean("RandomCraftEnabled", true);
            RANDOM_CRAFT_REFRESH_FEE = parser.getInt("RandomCraftRefreshFee", 50000);
            RANDOM_CRAFT_CREATE_FEE = parser.getInt("RandomCraftCreateFee", 300000);
            DROP_RANDOM_CRAFT_MATERIALS = parser.getBoolean("DropRandomCraftMaterials", true);
        }
    }
}