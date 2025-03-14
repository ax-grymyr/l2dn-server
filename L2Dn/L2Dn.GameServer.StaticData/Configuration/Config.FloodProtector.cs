using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class FloodProtector
    {
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_USE_ITEM = new("UseItemFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_ROLL_DICE = new("RollDiceFloodProtector");

        public static readonly FloodProtectorConfig
            FLOOD_PROTECTOR_ITEM_PET_SUMMON = new("ItemPetSummonFloodProtector");

        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_HERO_VOICE = new("HeroVoiceFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_GLOBAL_CHAT = new("GlobalChatFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_SUBCLASS = new("SubclassFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_DROP_ITEM = new("DropItemFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_SERVER_BYPASS = new("ServerBypassFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_MULTISELL = new("MultiSellFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_TRANSACTION = new("TransactionFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_MANUFACTURE = new("ManufactureFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_SENDMAIL = new("SendMailFloodProtector");

        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_CHARACTER_SELECT =
            new("CharacterSelectFloodProtector");

        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_ITEM_AUCTION = new("ItemAuctionFloodProtector");
        public static readonly FloodProtectorConfig FLOOD_PROTECTOR_PLAYER_ACTION = new("PlayerActionFloodProtector");

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.FloodProtector);

            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_USE_ITEM, "UseItem", 4);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_ROLL_DICE, "RollDice", 42);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_ITEM_PET_SUMMON, "ItemPetSummon", 16);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_HERO_VOICE, "HeroVoice", 100);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_GLOBAL_CHAT, "GlobalChat", 5);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_SUBCLASS, "Subclass", 20);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_DROP_ITEM, "DropItem", 10);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_SERVER_BYPASS, "ServerBypass", 5);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_MULTISELL, "MultiSell", 1);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_TRANSACTION, "Transaction", 10);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_MANUFACTURE, "Manufacture", 3);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_SENDMAIL, "SendMail", 100);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_CHARACTER_SELECT, "CharacterSelect", 30);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_ITEM_AUCTION, "ItemAuction", 9);
            LoadFloodProtectorConfig(parser, FLOOD_PROTECTOR_PLAYER_ACTION, "PlayerAction", 3);
        }

        /// <summary>
        /// Loads single flood protector configuration.
        /// </summary>
        /// <param name="parser">Configuration file reader.</param>
        /// <param name="config">Flood protector configuration instance.</param>
        /// <param name="configString">Flood protector configuration string that determines for which flood protector
        /// configuration should be read.</param>
        /// <param name="defaultInterval">Default flood protector interval.</param>
        private static void LoadFloodProtectorConfig(ConfigurationParser parser, FloodProtectorConfig config,
            string configString, int defaultInterval)
        {
            config.FLOOD_PROTECTION_INTERVAL =
                parser.getInt("FloodProtector" + configString + "Interval", defaultInterval);

            config.LOG_FLOODING = parser.getBoolean("FloodProtector" + configString + "LogFlooding");
            config.PUNISHMENT_LIMIT = parser.getInt("FloodProtector" + configString + "PunishmentLimit");
            config.PUNISHMENT_TYPE = parser.getString("FloodProtector" + configString + "PunishmentType", "none");
            config.PUNISHMENT_TIME = parser.getInt("FloodProtector" + configString + "PunishmentTime") * 60000;
        }
    }
}