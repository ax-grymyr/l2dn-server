using System.Globalization;
using L2Dn.Configuration;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class FactionSystem
    {
        public static bool FACTION_SYSTEM_ENABLED;
        public static Location FACTION_STARTING_LOCATION;
        public static Location FACTION_MANAGER_LOCATION;
        public static Location FACTION_GOOD_BASE_LOCATION;
        public static Location FACTION_EVIL_BASE_LOCATION;
        public static string FACTION_GOOD_TEAM_NAME = "Good";
        public static string FACTION_EVIL_TEAM_NAME = "Evil";
        public static Color FACTION_GOOD_NAME_COLOR;
        public static Color FACTION_EVIL_NAME_COLOR;
        public static bool FACTION_GUARDS_ENABLED;
        public static bool FACTION_RESPAWN_AT_BASE;
        public static bool FACTION_AUTO_NOBLESS;
        public static bool FACTION_SPECIFIC_CHAT;
        public static bool FACTION_BALANCE_ONLINE_PLAYERS;
        public static int FACTION_BALANCE_PLAYER_EXCEED_LIMIT;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.FactionSystem);

            FACTION_SYSTEM_ENABLED = parser.getBoolean("EnableFactionSystem");
            FACTION_STARTING_LOCATION = GetLocation(parser, "StartingLocation", 85332, 16199, -1252);
            FACTION_MANAGER_LOCATION = GetLocation(parser, "ManagerSpawnLocation", 85712, 15974, -1260, 26808);
            FACTION_GOOD_BASE_LOCATION = GetLocation(parser, "GoodBaseLocation", 45306, 48878, -3058);
            FACTION_EVIL_BASE_LOCATION = GetLocation(parser, "EvilBaseLocation", -44037, -113283, -237);
            FACTION_GOOD_TEAM_NAME = parser.getString("GoodTeamName", "Good");
            FACTION_EVIL_TEAM_NAME = parser.getString("EvilTeamName", "Evil");
            FACTION_GOOD_NAME_COLOR = parser.GetColor("GoodNameColor", new Color(0x00FF00));
            FACTION_EVIL_NAME_COLOR = parser.GetColor("EvilNameColor", new Color(0x0000FF));
            FACTION_GUARDS_ENABLED = parser.getBoolean("EnableFactionGuards", true);
            FACTION_RESPAWN_AT_BASE = parser.getBoolean("RespawnAtFactionBase", true);
            FACTION_AUTO_NOBLESS = parser.getBoolean("FactionAutoNobless");
            FACTION_SPECIFIC_CHAT = parser.getBoolean("EnableFactionChat", true);
            FACTION_BALANCE_ONLINE_PLAYERS = parser.getBoolean("BalanceOnlinePlayers", true);
            FACTION_BALANCE_PLAYER_EXCEED_LIMIT = parser.getInt("BalancePlayerExceedLimit", 20);
        }

        private static Location GetLocation(ConfigurationParser parser, string key, int dx, int dy, int dz,
            int dheading = 0)
        {
            string value = parser.getString(key);
            if (string.IsNullOrEmpty(value))
                return new Location(dx, dy, dz, dheading);

            string[] k = value.Split(',');
            if ((k.Length == 3 || k.Length == 4) && int.TryParse(k[0], CultureInfo.InvariantCulture, out int x) &&
                int.TryParse(k[1], CultureInfo.InvariantCulture, out int y) &&
                int.TryParse(k[2], CultureInfo.InvariantCulture, out int z))
            {
                if (k.Length == 4)
                {
                    if (int.TryParse(k[3], CultureInfo.InvariantCulture, out int heading))
                    {
                        return new Location(x, y, z, heading);
                    }
                }
                else
                {
                    return new Location(x, y, z, dheading);
                }
            }

            _logger.Error(
                $"Invalid location format '{value}' in entry '{key}' in configuration file '{parser.FilePath}'");

            return new Location(dx, dy, dz, dheading);
        }
    }
}