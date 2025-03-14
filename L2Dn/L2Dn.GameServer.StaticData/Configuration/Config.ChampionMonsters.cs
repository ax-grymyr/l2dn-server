using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class ChampionMonsters
    {
        public static bool CHAMPION_ENABLE;
        public static bool CHAMPION_PASSIVE;
        public static int CHAMPION_FREQUENCY;
        public static string CHAMP_TITLE = "Champion";
        public static bool SHOW_CHAMPION_AURA;
        public static int CHAMP_MIN_LEVEL;
        public static int CHAMP_MAX_LEVEL;
        public static int CHAMPION_HP;
        public static float CHAMPION_REWARDS_EXP_SP;
        public static float CHAMPION_REWARDS_CHANCE;
        public static float CHAMPION_REWARDS_AMOUNT;
        public static float CHAMPION_ADENAS_REWARDS_CHANCE;
        public static float CHAMPION_ADENAS_REWARDS_AMOUNT;
        public static float CHAMPION_HP_REGEN;
        public static float CHAMPION_ATK;
        public static float CHAMPION_SPD_ATK;
        public static int CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE;
        public static int CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE;
        public static ImmutableDictionary<int, int> CHAMPION_REWARD_ITEMS = ImmutableDictionary<int, int>.Empty;
        public static bool CHAMPION_ENABLE_VITALITY;
        public static bool CHAMPION_ENABLE_IN_INSTANCES;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.ChampionMonsters);

            CHAMPION_ENABLE = parser.getBoolean("ChampionEnable");
            CHAMPION_PASSIVE = parser.getBoolean("ChampionPassive");
            CHAMPION_FREQUENCY = parser.getInt("ChampionFrequency");
            CHAMP_TITLE = parser.getString("ChampionTitle", "Champion");
            SHOW_CHAMPION_AURA = parser.getBoolean("ChampionAura", true);
            CHAMP_MIN_LEVEL = parser.getInt("ChampionMinLevel", 20);
            CHAMP_MAX_LEVEL = parser.getInt("ChampionMaxLevel", 60);
            CHAMPION_HP = parser.getInt("ChampionHp", 7);
            CHAMPION_HP_REGEN = parser.getFloat("ChampionHpRegen", 1);
            CHAMPION_REWARDS_EXP_SP = parser.getFloat("ChampionRewardsExpSp", 8);
            CHAMPION_REWARDS_CHANCE = parser.getFloat("ChampionRewardsChance", 8);
            CHAMPION_REWARDS_AMOUNT = parser.getFloat("ChampionRewardsAmount", 1);
            CHAMPION_ADENAS_REWARDS_CHANCE = parser.getFloat("ChampionAdenasRewardsChance", 1);
            CHAMPION_ADENAS_REWARDS_AMOUNT = parser.getFloat("ChampionAdenasRewardsAmount", 1);
            CHAMPION_ATK = parser.getFloat("ChampionAtk", 1);
            CHAMPION_SPD_ATK = parser.getFloat("ChampionSpdAtk", 1);
            CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE = parser.getInt("ChampionRewardLowerLvlItemChance");
            CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE = parser.getInt("ChampionRewardHigherLvlItemChance");
            CHAMPION_REWARD_ITEMS = parser.GetIdValueMap<int>("ChampionRewardItems");
            CHAMPION_ENABLE_VITALITY = parser.getBoolean("ChampionEnableVitality");
            CHAMPION_ENABLE_IN_INSTANCES = parser.getBoolean("ChampionEnableInInstances");
        }
    }
}