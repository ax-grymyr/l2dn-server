using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class VipSystem
    {
        public static bool VIP_SYSTEM_ENABLED;
        public static bool VIP_SYSTEM_PRIME_AFFECT;
        public static bool VIP_SYSTEM_L_SHOP_AFFECT;
        public static int VIP_SYSTEM_MAX_TIER;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.VipSystem);

            VIP_SYSTEM_ENABLED = parser.getBoolean("VipEnabled");
            if (VIP_SYSTEM_ENABLED)
            {
                VIP_SYSTEM_PRIME_AFFECT = parser.getBoolean("PrimeAffectPoints");
                VIP_SYSTEM_L_SHOP_AFFECT = parser.getBoolean("LShopAffectPoints");
                VIP_SYSTEM_MAX_TIER = parser.getInt("MaxVipLevel", 7);
                if (VIP_SYSTEM_MAX_TIER > 10)
                    VIP_SYSTEM_MAX_TIER = 10;
            }
        }
    }
}