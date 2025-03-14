using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Pvp
    {
        public static bool KARMA_DROP_GM;
        public static int KARMA_PK_LIMIT;
        public static ImmutableSortedSet<int> KARMA_NONDROPPABLE_PET_ITEMS = ImmutableSortedSet<int>.Empty;
        public static ImmutableSortedSet<int> KARMA_NONDROPPABLE_ITEMS = ImmutableSortedSet<int>.Empty;
        public static bool ANTIFEED_ENABLE;
        public static bool ANTIFEED_DUALBOX;
        public static bool ANTIFEED_DISCONNECTED_AS_DUALBOX;
        public static int ANTIFEED_INTERVAL;
        public static bool VAMPIRIC_ATTACK_AFFECTS_PVP;
        public static bool MP_VAMPIRIC_ATTACK_AFFECTS_PVP;
        public static TimeSpan PVP_NORMAL_TIME;
        public static TimeSpan PVP_PVP_TIME;
        public static int MAX_REPUTATION;
        public static int REPUTATION_INCREASE;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Pvp);

            KARMA_DROP_GM = parser.getBoolean("CanGMDropEquipment");
            KARMA_PK_LIMIT = parser.getInt("MinimumPKRequiredToDrop", 4);
            KARMA_NONDROPPABLE_PET_ITEMS = parser.GetIntList("ListOfPetItems", ',', 2375, 3500, 3501, 3502, 4422, 4423,
                4424, 4425, 6648, 6649, 6650, 9882).ToImmutableSortedSet();

            KARMA_NONDROPPABLE_ITEMS = parser.GetIntList("ListOfNonDroppableItems", ',', 57, 1147, 425, 1146, 461, 10,
                2368, 7, 6, 2370, 2369, 6842, 6611, 6612, 6613, 6614, 6615, 6616, 6617, 6618, 6619, 6620, 6621, 7694,
                8181, 5575, 7694, 9388, 9389, 9390).ToImmutableSortedSet();

            ANTIFEED_ENABLE = parser.getBoolean("AntiFeedEnable");
            ANTIFEED_DUALBOX = parser.getBoolean("AntiFeedDualbox", true);
            ANTIFEED_DISCONNECTED_AS_DUALBOX = parser.getBoolean("AntiFeedDisconnectedAsDualbox", true);
            ANTIFEED_INTERVAL = parser.getInt("AntiFeedInterval", 120) * 1000;
            VAMPIRIC_ATTACK_AFFECTS_PVP = parser.getBoolean("VampiricAttackAffectsPvP");
            MP_VAMPIRIC_ATTACK_AFFECTS_PVP = parser.getBoolean("MpVampiricAttackAffectsPvP");
            PVP_NORMAL_TIME = TimeSpan.FromMilliseconds(parser.getInt("PvPVsNormalTime", 120000));
            PVP_PVP_TIME = TimeSpan.FromMilliseconds(parser.getInt("PvPVsPvPTime", 60000));
            MAX_REPUTATION = parser.getInt("MaxReputation", 500);
            REPUTATION_INCREASE = parser.getInt("ReputationIncrease", 100);
        }
    }
}