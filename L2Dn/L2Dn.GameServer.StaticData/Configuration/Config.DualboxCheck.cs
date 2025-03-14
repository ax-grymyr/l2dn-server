using System.Collections.Frozen;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class DualboxCheck
    {
        public static int DUALBOX_CHECK_MAX_PLAYERS_PER_IP;
        public static int DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP;
        public static int DUALBOX_CHECK_MAX_L2EVENT_PARTICIPANTS_PER_IP;
        public static bool DUALBOX_COUNT_OFFLINE_TRADERS;
        public static FrozenDictionary<int, int> DUALBOX_CHECK_WHITELIST = FrozenDictionary<int, int>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.DualboxCheck);

            DUALBOX_CHECK_MAX_PLAYERS_PER_IP = parser.getInt("DualboxCheckMaxPlayersPerIP");
            DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP = parser.getInt("DualboxCheckMaxOlympiadParticipantsPerIP");
            DUALBOX_CHECK_MAX_L2EVENT_PARTICIPANTS_PER_IP = parser.getInt("DualboxCheckMaxL2EventParticipantsPerIP");
            DUALBOX_COUNT_OFFLINE_TRADERS = parser.getBoolean("DualboxCountOfflineTraders");
            // DUALBOX_CHECK_WHITELIST = parser.getString("DualboxCheckWhitelist", "127.0.0.1,0"); // TODO: implement
        }
    }
}