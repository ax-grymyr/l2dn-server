using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class MagicLamp
    {
        public static bool ENABLE_MAGIC_LAMP;
        public static int MAGIC_LAMP_MAX_LEVEL_EXP;
        public static double MAGIC_LAMP_CHARGE_RATE;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.MagicLamp);

            ENABLE_MAGIC_LAMP = parser.getBoolean("MagicLampEnabled");
            MAGIC_LAMP_MAX_LEVEL_EXP = parser.getInt("MagicLampMaxLevelExp", 10000000);
            MAGIC_LAMP_CHARGE_RATE = parser.getDouble("MagicLampChargeRate", 0.1);
        }
    }
}