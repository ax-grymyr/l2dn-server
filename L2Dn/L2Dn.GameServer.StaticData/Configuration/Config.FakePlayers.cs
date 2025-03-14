using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class FakePlayers
    {
        public static bool FAKE_PLAYERS_ENABLED;
        public static bool FAKE_PLAYER_CHAT;
        public static bool FAKE_PLAYER_USE_SHOTS;
        public static bool FAKE_PLAYER_KILL_PVP;
        public static bool FAKE_PLAYER_KILL_KARMA;
        public static bool FAKE_PLAYER_AUTO_ATTACKABLE;
        public static bool FAKE_PLAYER_AGGRO_MONSTERS;
        public static bool FAKE_PLAYER_AGGRO_PLAYERS;
        public static bool FAKE_PLAYER_AGGRO_FPC;
        public static bool FAKE_PLAYER_CAN_DROP_ITEMS;
        public static bool FAKE_PLAYER_CAN_PICKUP;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.FakePlayers);

            FAKE_PLAYERS_ENABLED = parser.getBoolean("EnableFakePlayers");
            FAKE_PLAYER_CHAT = parser.getBoolean("FakePlayerChat");
            FAKE_PLAYER_USE_SHOTS = parser.getBoolean("FakePlayerUseShots");
            FAKE_PLAYER_KILL_PVP = parser.getBoolean("FakePlayerKillsRewardPvP");
            FAKE_PLAYER_KILL_KARMA = parser.getBoolean("FakePlayerUnflaggedKillsKarma");
            FAKE_PLAYER_AUTO_ATTACKABLE = parser.getBoolean("FakePlayerAutoAttackable");
            FAKE_PLAYER_AGGRO_MONSTERS = parser.getBoolean("FakePlayerAggroMonsters");
            FAKE_PLAYER_AGGRO_PLAYERS = parser.getBoolean("FakePlayerAggroPlayers");
            FAKE_PLAYER_AGGRO_FPC = parser.getBoolean("FakePlayerAggroFPC");
            FAKE_PLAYER_CAN_DROP_ITEMS = parser.getBoolean("FakePlayerCanDropItems");
            FAKE_PLAYER_CAN_PICKUP = parser.getBoolean("FakePlayerCanPickup");
        }
    }
}