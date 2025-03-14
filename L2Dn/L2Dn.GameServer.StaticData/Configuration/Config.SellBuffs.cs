using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class SellBuffs
    {
        public static bool SELLBUFF_ENABLED;
        public static int SELLBUFF_MP_MULTIPLER;
        public static int SELLBUFF_PAYMENT_ID;
        public static long SELLBUFF_MIN_PRICE;
        public static long SELLBUFF_MAX_PRICE;
        public static int SELLBUFF_MAX_BUFFS;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.SellBuffs);

            SELLBUFF_ENABLED = parser.getBoolean("SellBuffEnable");
            SELLBUFF_MP_MULTIPLER = parser.getInt("MpCostMultipler", 1);
            SELLBUFF_PAYMENT_ID = parser.getInt("PaymentID", 57);
            SELLBUFF_MIN_PRICE = parser.getLong("MinimumPrice", 100000);
            SELLBUFF_MAX_PRICE = parser.getLong("MaximumPrice", 100000000);
            SELLBUFF_MAX_BUFFS = parser.getInt("MaxBuffs", 15);
        }
    }
}