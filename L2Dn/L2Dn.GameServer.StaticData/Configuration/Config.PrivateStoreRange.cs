using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class PrivateStoreRange
    {
        public static int SHOP_MIN_RANGE_FROM_PLAYER;
        public static int SHOP_MIN_RANGE_FROM_NPC;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.PrivateStoreRange);

            SHOP_MIN_RANGE_FROM_PLAYER = parser.getInt("ShopMinRangeFromPlayer", 50);
            SHOP_MIN_RANGE_FROM_NPC = parser.getInt("ShopMinRangeFromNpc", 100);
        }
    }
}