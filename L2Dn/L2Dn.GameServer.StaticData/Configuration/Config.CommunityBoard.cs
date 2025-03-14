using System.Collections.Immutable;
using System.Globalization;
using L2Dn.Configuration;
using L2Dn.Extensions;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class CommunityBoard
    {
        public static bool CUSTOM_CB_ENABLED;
        public static int COMMUNITYBOARD_CURRENCY;
        public static bool COMMUNITYBOARD_ENABLE_MULTISELLS;
        public static bool COMMUNITYBOARD_ENABLE_TELEPORTS;
        public static bool COMMUNITYBOARD_ENABLE_BUFFS;
        public static bool COMMUNITYBOARD_ENABLE_HEAL;
        public static bool COMMUNITYBOARD_ENABLE_DELEVEL;
        public static int COMMUNITYBOARD_TELEPORT_PRICE;
        public static int COMMUNITYBOARD_BUFF_PRICE;
        public static int COMMUNITYBOARD_HEAL_PRICE;
        public static int COMMUNITYBOARD_DELEVEL_PRICE;
        public static bool COMMUNITYBOARD_COMBAT_DISABLED;
        public static bool COMMUNITYBOARD_KARMA_DISABLED;
        public static bool COMMUNITYBOARD_CAST_ANIMATIONS;
        public static bool COMMUNITY_PREMIUM_SYSTEM_ENABLED;
        public static int COMMUNITY_PREMIUM_COIN_ID;
        public static int COMMUNITY_PREMIUM_PRICE_PER_DAY;
        public static ImmutableSortedSet<int> COMMUNITY_AVAILABLE_BUFFS = ImmutableSortedSet<int>.Empty;

        public static ImmutableDictionary<string, Location> COMMUNITY_AVAILABLE_TELEPORTS =
            ImmutableDictionary<string, Location>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.CommunityBoard);

            CUSTOM_CB_ENABLED = parser.getBoolean("CustomCommunityBoard");
            COMMUNITYBOARD_CURRENCY = parser.getInt("CommunityCurrencyId", 57);
            COMMUNITYBOARD_ENABLE_MULTISELLS = parser.getBoolean("CommunityEnableMultisells", true);
            COMMUNITYBOARD_ENABLE_TELEPORTS = parser.getBoolean("CommunityEnableTeleports", true);
            COMMUNITYBOARD_ENABLE_BUFFS = parser.getBoolean("CommunityEnableBuffs", true);
            COMMUNITYBOARD_ENABLE_HEAL = parser.getBoolean("CommunityEnableHeal", true);
            COMMUNITYBOARD_ENABLE_DELEVEL = parser.getBoolean("CommunityEnableDelevel");
            COMMUNITYBOARD_TELEPORT_PRICE = parser.getInt("CommunityTeleportPrice");
            COMMUNITYBOARD_BUFF_PRICE = parser.getInt("CommunityBuffPrice");
            COMMUNITYBOARD_HEAL_PRICE = parser.getInt("CommunityHealPrice");
            COMMUNITYBOARD_DELEVEL_PRICE = parser.getInt("CommunityDelevelPrice");
            COMMUNITYBOARD_KARMA_DISABLED = parser.getBoolean("CommunityKarmaDisabled", true);
            COMMUNITYBOARD_CAST_ANIMATIONS = parser.getBoolean("CommunityCastAnimations");
            COMMUNITY_PREMIUM_SYSTEM_ENABLED = parser.getBoolean("CommunityPremiumSystem");
            COMMUNITY_PREMIUM_COIN_ID = parser.getInt("CommunityPremiumBuyCoinId", 57);
            COMMUNITY_PREMIUM_PRICE_PER_DAY = parser.getInt("CommunityPremiumPricePerDay", 1000000);
            COMMUNITY_AVAILABLE_BUFFS = parser.GetIntList("CommunityAvailableBuffs").ToImmutableSortedSet();
            COMMUNITY_AVAILABLE_TELEPORTS = GetLocations(parser, "CommunityTeleportList");
        }

        private static ImmutableDictionary<string, Location> GetLocations(ConfigurationParser parser, string key)
        {
            // Format:
            // TeleportName1,X1,Y1,Z1;TeleportName2,X2,Y2,Z2...

            ImmutableDictionary<string, Location>.Builder builder =
                ImmutableDictionary<string, Location>.Empty.ToBuilder();

            parser.GetList(key, ';', s =>
            {
                string[] item = s.Split(',');
                int x;
                int y = 0;
                int z = 0;
                bool ok = int.TryParse(item[1], CultureInfo.InvariantCulture, out x) &&
                    int.TryParse(item[2], CultureInfo.InvariantCulture, out y) &&
                    int.TryParse(item[3], CultureInfo.InvariantCulture, out z);

                return ((Name: item[0], Location: new Location(x, y, z, 0)), ok);
            }, true).ForEach(tuple =>
            {
                try
                {
                    builder.Add(tuple.Name, tuple.Location);
                }
                catch (ArgumentException)
                {
                    _logger.Error(
                        $"Duplicated location name '{tuple.Name}' in entry '{key}' in configuration file '{parser.FilePath}'");
                }

            });

            return builder.ToImmutable();
        }
    }
}