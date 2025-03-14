using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    /// <summary>
    /// Server Settings
    /// </summary>
    public static class Server
    {
        private static readonly Regex _defaultRegex = new(".*", RegexOptions.Compiled | RegexOptions.NonBacktracking);

        public static Regex CHARNAME_TEMPLATE_PATTERN = _defaultRegex;
        public static Regex PET_NAME_TEMPLATE = _defaultRegex;
        public static Regex CLAN_NAME_TEMPLATE = _defaultRegex;

        public static int MAX_CHARACTERS_NUMBER_PER_ACCOUNT;
        public static int MAXIMUM_ONLINE_USERS;

        public static bool HARDWARE_INFO_ENABLED;
        public static bool KICK_MISSING_HWID;
        public static int MAX_PLAYERS_PER_HWID;

        public static FrozenSet<int> PROTOCOL_LIST = FrozenSet<int>.Empty;
        public static GameServerType SERVER_LIST_TYPE;
        public static int SERVER_LIST_AGE;
        public static bool SERVER_LIST_BRACKET;

        public static int SCHEDULED_THREAD_POOL_SIZE;
        public static int INSTANT_THREAD_POOL_SIZE;
        public static bool THREADS_FOR_LOADING;

        public static bool DEADLOCK_DETECTOR;
        public static int DEADLOCK_CHECK_INTERVAL;
        public static bool RESTART_ON_DEADLOCK;

        public static bool SERVER_RESTART_SCHEDULE_ENABLED;
        public static bool SERVER_RESTART_SCHEDULE_MESSAGE;
        public static int SERVER_RESTART_SCHEDULE_COUNTDOWN;
        public static ImmutableArray<TimeOnly> SERVER_RESTART_SCHEDULE = ImmutableArray<TimeOnly>.Empty;
        public static ImmutableArray<DayOfWeek> SERVER_RESTART_DAYS = ImmutableArray<DayOfWeek>.Empty;

        public static bool PRECAUTIONARY_RESTART_ENABLED;
        public static bool PRECAUTIONARY_RESTART_CPU;
        public static bool PRECAUTIONARY_RESTART_MEMORY;
        public static bool PRECAUTIONARY_RESTART_CHECKS;
        public static int PRECAUTIONARY_RESTART_PERCENTAGE;
        public static int PRECAUTIONARY_RESTART_DELAY;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Server);

            CHARNAME_TEMPLATE_PATTERN = parser.GetRegex("CnameTemplate", _defaultRegex);
            PET_NAME_TEMPLATE = parser.GetRegex("PetNameTemplate", _defaultRegex);
            CLAN_NAME_TEMPLATE = parser.GetRegex("ClanNameTemplate", _defaultRegex);
            MAX_CHARACTERS_NUMBER_PER_ACCOUNT = parser.getInt("CharMaxNumber", 7);
            MAXIMUM_ONLINE_USERS = parser.getInt("MaximumOnlineUsers", 100);
            HARDWARE_INFO_ENABLED = parser.getBoolean("EnableHardwareInfo");
            KICK_MISSING_HWID = parser.getBoolean("KickMissingHWID");
            MAX_PLAYERS_PER_HWID = parser.getInt("MaxPlayersPerHWID");
            if (MAX_PLAYERS_PER_HWID > 0)
                KICK_MISSING_HWID = true;

            PROTOCOL_LIST = parser.GetSet("AllowedProtocolRevisions", ';', 447);
            SERVER_LIST_TYPE = parser.GetEnum("ServerListType", GameServerType.Classic);
            SERVER_LIST_AGE = parser.getInt("ServerListAge");
            SERVER_LIST_BRACKET = parser.getBoolean("ServerListBrackets");
            SCHEDULED_THREAD_POOL_SIZE = parser.getInt("ScheduledThreadPoolSize", Environment.ProcessorCount * 4);
            INSTANT_THREAD_POOL_SIZE = parser.getInt("InstantThreadPoolSize", Environment.ProcessorCount * 2);
            THREADS_FOR_LOADING = parser.getBoolean("ThreadsForLoading");
            DEADLOCK_DETECTOR = parser.getBoolean("DeadLockDetector", true);
            DEADLOCK_CHECK_INTERVAL = parser.getInt("DeadLockCheckInterval", 20);
            RESTART_ON_DEADLOCK = parser.getBoolean("RestartOnDeadlock");
            SERVER_RESTART_SCHEDULE_ENABLED = parser.getBoolean("ServerRestartScheduleEnabled");
            SERVER_RESTART_SCHEDULE_MESSAGE = parser.getBoolean("ServerRestartScheduleMessage");
            SERVER_RESTART_SCHEDULE_COUNTDOWN = parser.getInt("ServerRestartScheduleCountdown", 600);
            SERVER_RESTART_SCHEDULE =
                parser.GetTimeList("ServerRestartSchedule", ',', TimeOnly.FromTimeSpan(TimeSpan.FromHours(8)));

            SERVER_RESTART_DAYS = parser.GetIntList("ServerRestartDays").Select(x => (DayOfWeek)((x - 1) % 7)).
                ToImmutableArray();

            PRECAUTIONARY_RESTART_ENABLED = parser.getBoolean("PrecautionaryRestartEnabled");
            PRECAUTIONARY_RESTART_CPU = parser.getBoolean("PrecautionaryRestartCpu", true);
            PRECAUTIONARY_RESTART_MEMORY = parser.getBoolean("PrecautionaryRestartMemory");
            PRECAUTIONARY_RESTART_CHECKS = parser.getBoolean("PrecautionaryRestartChecks", true);
            PRECAUTIONARY_RESTART_PERCENTAGE = parser.getInt("PrecautionaryRestartPercentage", 95);
            PRECAUTIONARY_RESTART_DELAY = parser.getInt("PrecautionaryRestartDelay", 60) * 1000;
        }
    }
}