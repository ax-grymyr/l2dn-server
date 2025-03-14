using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class MagicLamp
    {
        public static bool ENABLE_MAGIC_LAMP;
        public static int MAGIC_LAMP_MAX_LEVEL_EXP;
        public static double MAGIC_LAMP_CHARGE_RATE;

        public static void Load(string configPath)
        {
            ConfigurationParser parser = new(configPath);
            parser.LoadConfig(FileNames.Configs.AttendanceConfigFile);

            ENABLE_MAGIC_LAMP = parser.getBoolean("MagicLampEnabled");
            MAGIC_LAMP_MAX_LEVEL_EXP = parser.getInt("MagicLampMaxLevelExp", 10000000);
            MAGIC_LAMP_CHARGE_RATE = parser.getDouble("MagicLampChargeRate", 0.1);
        }
    }
}