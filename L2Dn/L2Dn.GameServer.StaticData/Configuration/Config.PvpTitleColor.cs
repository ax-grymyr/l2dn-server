using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class PvpTitleColor
    {
        public static bool PVP_COLOR_SYSTEM_ENABLED;
        public static int PVP_AMOUNT1;
        public static int PVP_AMOUNT2;
        public static int PVP_AMOUNT3;
        public static int PVP_AMOUNT4;
        public static int PVP_AMOUNT5;
        public static Color NAME_COLOR_FOR_PVP_AMOUNT1;
        public static Color NAME_COLOR_FOR_PVP_AMOUNT2;
        public static Color NAME_COLOR_FOR_PVP_AMOUNT3;
        public static Color NAME_COLOR_FOR_PVP_AMOUNT4;
        public static Color NAME_COLOR_FOR_PVP_AMOUNT5;
        public static string TITLE_FOR_PVP_AMOUNT1 = "Title";
        public static string TITLE_FOR_PVP_AMOUNT2 = "Title";
        public static string TITLE_FOR_PVP_AMOUNT3 = "Title";
        public static string TITLE_FOR_PVP_AMOUNT4 = "Title";
        public static string TITLE_FOR_PVP_AMOUNT5 = "Title";

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.PvpTitleColor);

            PVP_COLOR_SYSTEM_ENABLED = parser.getBoolean("EnablePvPColorSystem");
            PVP_AMOUNT1 = parser.getInt("PvpAmount1", 500);
            PVP_AMOUNT2 = parser.getInt("PvpAmount2", 1000);
            PVP_AMOUNT3 = parser.getInt("PvpAmount3", 1500);
            PVP_AMOUNT4 = parser.getInt("PvpAmount4", 2500);
            PVP_AMOUNT5 = parser.getInt("PvpAmount5", 5000);
            NAME_COLOR_FOR_PVP_AMOUNT1 = parser.GetColor("ColorForAmount1", new Color(0x00FF00));
            NAME_COLOR_FOR_PVP_AMOUNT2 = parser.GetColor("ColorForAmount2", new Color(0x00FF00));
            NAME_COLOR_FOR_PVP_AMOUNT3 = parser.GetColor("ColorForAmount3", new Color(0x00FF00));
            NAME_COLOR_FOR_PVP_AMOUNT4 = parser.GetColor("ColorForAmount4", new Color(0x00FF00));
            NAME_COLOR_FOR_PVP_AMOUNT5 = parser.GetColor("ColorForAmount5", new Color(0x00FF00));
            TITLE_FOR_PVP_AMOUNT1 = parser.getString("PvPTitleForAmount1", "Title");
            TITLE_FOR_PVP_AMOUNT2 = parser.getString("PvPTitleForAmount2", "Title");
            TITLE_FOR_PVP_AMOUNT3 = parser.getString("PvPTitleForAmount3", "Title");
            TITLE_FOR_PVP_AMOUNT4 = parser.getString("PvPTitleForAmount4", "Title");
            TITLE_FOR_PVP_AMOUNT5 = parser.getString("PvPTitleForAmount5", "Title");
        }
    }
}