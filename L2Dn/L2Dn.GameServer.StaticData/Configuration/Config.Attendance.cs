using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Attendance
    {
        public static bool ENABLE_ATTENDANCE_REWARDS;
        public static bool PREMIUM_ONLY_ATTENDANCE_REWARDS;
        public static bool VIP_ONLY_ATTENDANCE_REWARDS;
        public static bool ATTENDANCE_REWARDS_SHARE_ACCOUNT;
        public static TimeSpan ATTENDANCE_REWARD_DELAY;
        public static bool ATTENDANCE_POPUP_START;
        public static bool ATTENDANCE_POPUP_WINDOW;

        public static void Load(string configPath)
        {
            ConfigurationParser parser = new(configPath);
            parser.LoadConfig(FileNames.Configs.AttendanceConfigFile);

            ENABLE_ATTENDANCE_REWARDS = parser.getBoolean("EnableAttendanceRewards");
            PREMIUM_ONLY_ATTENDANCE_REWARDS = parser.getBoolean("PremiumOnlyAttendanceRewards");
            VIP_ONLY_ATTENDANCE_REWARDS = parser.getBoolean("VipOnlyAttendanceRewards");
            ATTENDANCE_REWARDS_SHARE_ACCOUNT = parser.getBoolean("AttendanceRewardsShareAccount");
            ATTENDANCE_REWARD_DELAY = TimeSpan.FromMinutes(parser.getInt("AttendanceRewardDelay", 30));
            ATTENDANCE_POPUP_START = parser.getBoolean("AttendancePopupStart", true);
            ATTENDANCE_POPUP_WINDOW = parser.getBoolean("AttendancePopupWindow");
        }
    }
}