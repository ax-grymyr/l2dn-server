using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class AutoPotions
    {
        public static bool AUTO_POTIONS_ENABLED;
        public static bool AUTO_POTIONS_IN_OLYMPIAD;
        public static int AUTO_POTION_MIN_LEVEL;
        public static bool AUTO_CP_ENABLED;
        public static bool AUTO_HP_ENABLED;
        public static bool AUTO_MP_ENABLED;
        public static int AUTO_CP_PERCENTAGE;
        public static int AUTO_HP_PERCENTAGE;
        public static int AUTO_MP_PERCENTAGE;
        public static ImmutableSortedSet<int> AUTO_CP_ITEM_IDS = ImmutableSortedSet<int>.Empty;
        public static ImmutableSortedSet<int> AUTO_HP_ITEM_IDS = ImmutableSortedSet<int>.Empty;
        public static ImmutableSortedSet<int> AUTO_MP_ITEM_IDS = ImmutableSortedSet<int>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.AutoPotions);

            AUTO_POTIONS_ENABLED = parser.getBoolean("AutoPotionsEnabled");
            AUTO_POTIONS_IN_OLYMPIAD = parser.getBoolean("AutoPotionsInOlympiad");
            AUTO_POTION_MIN_LEVEL = parser.getInt("AutoPotionMinimumLevel", 1);
            AUTO_CP_ENABLED = parser.getBoolean("AutoCpEnabled", true);
            AUTO_HP_ENABLED = parser.getBoolean("AutoHpEnabled", true);
            AUTO_MP_ENABLED = parser.getBoolean("AutoMpEnabled", true);
            AUTO_CP_PERCENTAGE = parser.getInt("AutoCpPercentage", 70);
            AUTO_HP_PERCENTAGE = parser.getInt("AutoHpPercentage", 70);
            AUTO_MP_PERCENTAGE = parser.getInt("AutoMpPercentage", 70);
            AUTO_CP_ITEM_IDS = parser.GetIntList("AutoCpItemIds").ToImmutableSortedSet();
            AUTO_HP_ITEM_IDS = parser.GetIntList("AutoHpItemIds").ToImmutableSortedSet();
            AUTO_MP_ITEM_IDS = parser.GetIntList("AutoMpItemIds").ToImmutableSortedSet();
        }
    }
}