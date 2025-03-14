using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class WorldExchange
    {
        public static bool ENABLE_WORLD_EXCHANGE;
        public static string WORLD_EXCHANGE_DEFAULT_LANG = "en";
        public static long WORLD_EXCHANGE_SAVE_INTERVAL;
        public static double WORLD_EXCHANGE_LCOIN_TAX;
        public static long WORLD_EXCHANGE_MAX_LCOIN_TAX;
        public static double WORLD_EXCHANGE_ADENA_FEE;
        public static long WORLD_EXCHANGE_MAX_ADENA_FEE;
        public static bool WORLD_EXCHANGE_LAZY_UPDATE;
        public static int WORLD_EXCHANGE_ITEM_SELL_PERIOD;
        public static int WORLD_EXCHANGE_ITEM_BACK_PERIOD;
        public static int WORLD_EXCHANGE_PAYMENT_TAKE_PERIOD;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.WorldExchange);

            ENABLE_WORLD_EXCHANGE = parser.getBoolean("EnableWorldExchange", true);
            WORLD_EXCHANGE_DEFAULT_LANG = parser.getString("WorldExchangeDefaultLanguage", "en");
            WORLD_EXCHANGE_SAVE_INTERVAL = parser.getLong("BidItemsIntervalStatusCheck", 30000);
            WORLD_EXCHANGE_LCOIN_TAX = parser.getDouble("LCoinFee", 0.05);
            WORLD_EXCHANGE_MAX_LCOIN_TAX = parser.getLong("MaxLCoinFee", 20000);
            WORLD_EXCHANGE_ADENA_FEE = parser.getDouble("AdenaFee", 100.0);
            WORLD_EXCHANGE_MAX_ADENA_FEE = parser.getLong("MaxAdenaFee", -1);
            WORLD_EXCHANGE_LAZY_UPDATE = parser.getBoolean("DBLazy");
            WORLD_EXCHANGE_ITEM_SELL_PERIOD = parser.getInt("ItemSellPeriod", 14);
            WORLD_EXCHANGE_ITEM_BACK_PERIOD = parser.getInt("ItemBackPeriod", 120);
            WORLD_EXCHANGE_PAYMENT_TAKE_PERIOD = parser.getInt("PaymentTakePeriod", 120);
        }
    }
}