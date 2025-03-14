using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class PvpAnnounce
    {
        public static bool ANNOUNCE_PK_PVP;
        public static bool ANNOUNCE_PK_PVP_NORMAL_MESSAGE;
        public static string ANNOUNCE_PK_MSG = "$killer has slaughtered $target";
        public static string ANNOUNCE_PVP_MSG = "$killer has defeated $target";

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.PvpAnnounce);

            ANNOUNCE_PK_PVP = parser.getBoolean("AnnouncePkPvP");
            ANNOUNCE_PK_PVP_NORMAL_MESSAGE = parser.getBoolean("AnnouncePkPvPNormalMessage", true);
            ANNOUNCE_PK_MSG = parser.getString("AnnouncePkMsg", "$killer has slaughtered $target");
            ANNOUNCE_PVP_MSG = parser.getString("AnnouncePvpMsg", "$killer has defeated $target");
        }
    }
}