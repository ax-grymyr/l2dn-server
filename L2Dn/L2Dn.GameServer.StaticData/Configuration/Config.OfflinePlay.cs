using System.Collections.Immutable;
using L2Dn.Configuration;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class OfflinePlay
    {
        public static bool ENABLE_OFFLINE_PLAY_COMMAND;
        public static bool OFFLINE_PLAY_PREMIUM;
        public static bool OFFLINE_PLAY_LOGOUT_ON_DEATH;
        public static string OFFLINE_PLAY_LOGIN_MESSAGE = string.Empty;
        public static bool OFFLINE_PLAY_SET_NAME_COLOR;
        public static Color OFFLINE_PLAY_NAME_COLOR;

        public static ImmutableArray<AbnormalVisualEffect> OFFLINE_PLAY_ABNORMAL_EFFECTS =
            ImmutableArray<AbnormalVisualEffect>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.OfflinePlay);

            ENABLE_OFFLINE_PLAY_COMMAND = parser.getBoolean("EnableOfflinePlayCommand");
            OFFLINE_PLAY_PREMIUM = parser.getBoolean("OfflinePlayPremium");
            OFFLINE_PLAY_LOGOUT_ON_DEATH = parser.getBoolean("OfflinePlayLogoutOnDeath", true);
            OFFLINE_PLAY_LOGIN_MESSAGE = parser.getString("OfflinePlayLoginMessage");
            OFFLINE_PLAY_SET_NAME_COLOR = parser.getBoolean("OfflinePlaySetNameColor");
            OFFLINE_PLAY_NAME_COLOR = parser.GetColor("OfflinePlayNameColor", new Color(0x808080));
            OFFLINE_PLAY_ABNORMAL_EFFECTS = parser.GetEnumList<AbnormalVisualEffect>("OfflinePlayAbnormalEffect");
        }
    }
}