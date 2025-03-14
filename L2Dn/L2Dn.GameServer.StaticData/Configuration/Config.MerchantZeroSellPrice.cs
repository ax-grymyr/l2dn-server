using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class MerchantZeroSellPrice
    {
        public static bool MERCHANT_ZERO_SELL_PRICE;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.MerchantZeroSellPrice);

            MERCHANT_ZERO_SELL_PRICE = parser.getBoolean("MerchantZeroSellPrice");
        }
    }
}