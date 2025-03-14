using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Banking
    {
        public static bool BANKING_SYSTEM_ENABLED;
        public static int BANKING_SYSTEM_GOLDBARS;
        public static int BANKING_SYSTEM_ADENA;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Banking);

            BANKING_SYSTEM_ENABLED = parser.getBoolean("BankingEnabled");
            BANKING_SYSTEM_GOLDBARS = parser.getInt("BankingGoldbarCount", 1);
            BANKING_SYSTEM_ADENA = parser.getInt("BankingAdenaCount", 500000000);
        }
    }
}