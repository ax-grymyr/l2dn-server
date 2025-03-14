using System.Collections.Immutable;
using L2Dn.Configuration;
using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Olympiad
    {
        public static bool OLYMPIAD_ENABLED;
        public static int ALT_OLY_START_TIME;
        public static int ALT_OLY_MIN;
        public static long ALT_OLY_CPERIOD;
        public static long ALT_OLY_BATTLE;
        public static long ALT_OLY_WPERIOD;
        public static long ALT_OLY_VPERIOD;
        public static int ALT_OLY_START_POINTS;
        public static int ALT_OLY_WEEKLY_POINTS;
        public static int ALT_OLY_CLASSED;
        public static int ALT_OLY_NONCLASSED;
        public static ImmutableArray<ItemHolder> ALT_OLY_WINNER_REWARD = ImmutableArray<ItemHolder>.Empty;
        public static ImmutableArray<ItemHolder> ALT_OLY_LOSER_REWARD = ImmutableArray<ItemHolder>.Empty;
        public static int ALT_OLY_COMP_RITEM;
        public static int ALT_OLY_MIN_MATCHES;
        public static int ALT_OLY_MARK_PER_POINT;
        public static int ALT_OLY_HERO_POINTS;
        public static int ALT_OLY_RANK1_POINTS;
        public static int ALT_OLY_RANK2_POINTS;
        public static int ALT_OLY_RANK3_POINTS;
        public static int ALT_OLY_RANK4_POINTS;
        public static int ALT_OLY_RANK5_POINTS;
        public static int ALT_OLY_MAX_POINTS;
        public static int ALT_OLY_DIVIDER_CLASSED;
        public static int ALT_OLY_DIVIDER_NON_CLASSED;
        public static int ALT_OLY_MAX_WEEKLY_MATCHES;
        public static bool ALT_OLY_LOG_FIGHTS;
        public static bool ALT_OLY_SHOW_MONTHLY_WINNERS;
        public static bool ALT_OLY_ANNOUNCE_GAMES;
        public static ImmutableSortedSet<int> LIST_OLY_RESTRICTED_ITEMS = ImmutableSortedSet<int>.Empty;
        public static int ALT_OLY_WEAPON_ENCHANT_LIMIT;
        public static int ALT_OLY_ARMOR_ENCHANT_LIMIT;
        public static int ALT_OLY_WAIT_TIME;
        public static string ALT_OLY_PERIOD = "MONTH";
        public static int ALT_OLY_PERIOD_MULTIPLIER;
        public static ImmutableSortedSet<DayOfWeek> ALT_OLY_COMPETITION_DAYS = ImmutableSortedSet<DayOfWeek>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Olympiad);

            OLYMPIAD_ENABLED = parser.getBoolean("OlympiadEnabled", true);
            ALT_OLY_START_TIME = parser.getInt("AltOlyStartTime", 20);
            ALT_OLY_MIN = parser.getInt("AltOlyMin");
            ALT_OLY_CPERIOD = parser.getLong("AltOlyCPeriod", 14400000);
            ALT_OLY_BATTLE = parser.getLong("AltOlyBattle", 300000);
            ALT_OLY_WPERIOD = parser.getLong("AltOlyWPeriod", 604800000);
            ALT_OLY_VPERIOD = parser.getLong("AltOlyVPeriod", 86400000);
            ALT_OLY_START_POINTS = parser.getInt("AltOlyStartPoints", 10);
            ALT_OLY_WEEKLY_POINTS = parser.getInt("AltOlyWeeklyPoints", 10);
            ALT_OLY_CLASSED = parser.getInt("AltOlyClassedParticipants", 10);
            ALT_OLY_NONCLASSED = parser.getInt("AltOlyNonClassedParticipants", 20);
            ALT_OLY_WINNER_REWARD = parser.GetIdValueMap<int>("AltOlyWinReward").
                Select(x => new ItemHolder(x.Key, x.Value)).ToImmutableArray();

            ALT_OLY_LOSER_REWARD = parser.GetIdValueMap<int>("AltOlyLoserReward").
                Select(x => new ItemHolder(x.Key, x.Value)).ToImmutableArray();

            ALT_OLY_COMP_RITEM = parser.getInt("AltOlyCompRewItem", 45584);
            ALT_OLY_MIN_MATCHES = parser.getInt("AltOlyMinMatchesForPoints", 10);
            ALT_OLY_MARK_PER_POINT = parser.getInt("AltOlyMarkPerPoint", 20);
            ALT_OLY_HERO_POINTS = parser.getInt("AltOlyHeroPoints", 30);
            ALT_OLY_RANK1_POINTS = parser.getInt("AltOlyRank1Points", 60);
            ALT_OLY_RANK2_POINTS = parser.getInt("AltOlyRank2Points", 50);
            ALT_OLY_RANK3_POINTS = parser.getInt("AltOlyRank3Points", 45);
            ALT_OLY_RANK4_POINTS = parser.getInt("AltOlyRank4Points", 40);
            ALT_OLY_RANK5_POINTS = parser.getInt("AltOlyRank5Points", 30);
            ALT_OLY_MAX_POINTS = parser.getInt("AltOlyMaxPoints", 10);
            ALT_OLY_DIVIDER_CLASSED = parser.getInt("AltOlyDividerClassed", 5);
            ALT_OLY_DIVIDER_NON_CLASSED = parser.getInt("AltOlyDividerNonClassed", 5);
            ALT_OLY_MAX_WEEKLY_MATCHES = parser.getInt("AltOlyMaxWeeklyMatches", 30);
            ALT_OLY_LOG_FIGHTS = parser.getBoolean("AltOlyLogFights");
            ALT_OLY_SHOW_MONTHLY_WINNERS = parser.getBoolean("AltOlyShowMonthlyWinners", true);
            ALT_OLY_ANNOUNCE_GAMES = parser.getBoolean("AltOlyAnnounceGames", true);
            LIST_OLY_RESTRICTED_ITEMS = parser.GetIntList("AltOlyRestrictedItems").ToImmutableSortedSet();
            ALT_OLY_WEAPON_ENCHANT_LIMIT = parser.getInt("AltOlyWeaponEnchantLimit", -1);
            ALT_OLY_ARMOR_ENCHANT_LIMIT = parser.getInt("AltOlyArmorEnchantLimit", -1);
            ALT_OLY_WAIT_TIME = parser.getInt("AltOlyWaitTime", 60);
            ALT_OLY_PERIOD = parser.getString("AltOlyPeriod", "MONTH");
            ALT_OLY_PERIOD_MULTIPLIER = parser.getInt("AltOlyPeriodMultiplier", 1);
            ALT_OLY_COMPETITION_DAYS = parser.GetIntList("AltOlyCompetitionDays").Select(x => (DayOfWeek)x).
                ToImmutableSortedSet();
        }
    }
}