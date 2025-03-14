using System.Collections.Immutable;
using L2Dn.Configuration;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class OfflineTrade
    {
        public static bool OFFLINE_TRADE_ENABLE;
        public static bool OFFLINE_CRAFT_ENABLE;
        public static bool OFFLINE_MODE_IN_PEACE_ZONE;
        public static bool OFFLINE_MODE_NO_DAMAGE;
        public static bool RESTORE_OFFLINERS;
        public static int OFFLINE_MAX_DAYS;
        public static bool OFFLINE_DISCONNECT_FINISHED;
        public static bool OFFLINE_DISCONNECT_SAME_ACCOUNT;
        public static bool OFFLINE_SET_NAME_COLOR;
        public static Color OFFLINE_NAME_COLOR;
        public static bool OFFLINE_FAME;
        public static bool STORE_OFFLINE_TRADE_IN_REALTIME;
        public static bool ENABLE_OFFLINE_COMMAND;

        public static ImmutableArray<AbnormalVisualEffect> OFFLINE_ABNORMAL_EFFECTS =
            ImmutableArray<AbnormalVisualEffect>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.OfflineTrade);

            OFFLINE_TRADE_ENABLE = parser.getBoolean("OfflineTradeEnable");
            OFFLINE_CRAFT_ENABLE = parser.getBoolean("OfflineCraftEnable");
            OFFLINE_MODE_IN_PEACE_ZONE = parser.getBoolean("OfflineModeInPeaceZone");
            OFFLINE_MODE_NO_DAMAGE = parser.getBoolean("OfflineModeNoDamage");
            OFFLINE_SET_NAME_COLOR = parser.getBoolean("OfflineSetNameColor");
            OFFLINE_NAME_COLOR = parser.GetColor("OfflineNameColor", new Color(0x808080));
            OFFLINE_FAME = parser.getBoolean("OfflineFame", true);
            RESTORE_OFFLINERS = parser.getBoolean("RestoreOffliners");
            OFFLINE_MAX_DAYS = parser.getInt("OfflineMaxDays", 10);
            OFFLINE_DISCONNECT_FINISHED = parser.getBoolean("OfflineDisconnectFinished", true);
            OFFLINE_DISCONNECT_SAME_ACCOUNT = parser.getBoolean("OfflineDisconnectSameAccount");
            STORE_OFFLINE_TRADE_IN_REALTIME = parser.getBoolean("StoreOfflineTradeInRealtime", true);
            ENABLE_OFFLINE_COMMAND = parser.getBoolean("EnableOfflineCommand", true);
            OFFLINE_ABNORMAL_EFFECTS = parser.GetEnumList<AbnormalVisualEffect>("OfflineAbnormalEffect");
        }
    }
}