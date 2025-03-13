using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Feature
    {
        // Castle Settings
        public static ImmutableArray<int> SIEGE_HOUR_LIST = ImmutableArray<int>.Empty;
        public static int CASTLE_BUY_TAX_NEUTRAL;
        public static int CASTLE_BUY_TAX_LIGHT;
        public static int CASTLE_BUY_TAX_DARK;
        public static int CASTLE_SELL_TAX_NEUTRAL;
        public static int CASTLE_SELL_TAX_LIGHT;
        public static int CASTLE_SELL_TAX_DARK;
        public static long CS_TELE_FEE_RATIO;
        public static int CS_TELE1_FEE;
        public static int CS_TELE2_FEE;
        public static long CS_SUPPORT_FEE_RATIO;
        public static int CS_SUPPORT1_FEE;
        public static int CS_SUPPORT2_FEE;
        public static long CS_MPREG_FEE_RATIO;
        public static int CS_MPREG1_FEE;
        public static int CS_MPREG2_FEE;
        public static long CS_HPREG_FEE_RATIO;
        public static int CS_HPREG1_FEE;
        public static int CS_HPREG2_FEE;
        public static long CS_EXPREG_FEE_RATIO;
        public static int CS_EXPREG1_FEE;
        public static int CS_EXPREG2_FEE;
        public static int OUTER_DOOR_UPGRADE_PRICE2;
        public static int OUTER_DOOR_UPGRADE_PRICE3;
        public static int OUTER_DOOR_UPGRADE_PRICE5;
        public static int INNER_DOOR_UPGRADE_PRICE2;
        public static int INNER_DOOR_UPGRADE_PRICE3;
        public static int INNER_DOOR_UPGRADE_PRICE5;
        public static int WALL_UPGRADE_PRICE2;
        public static int WALL_UPGRADE_PRICE3;
        public static int WALL_UPGRADE_PRICE5;
        public static int TRAP_UPGRADE_PRICE1;
        public static int TRAP_UPGRADE_PRICE2;
        public static int TRAP_UPGRADE_PRICE3;
        public static int TRAP_UPGRADE_PRICE4;

        // Fortress Settings
        public static TimeSpan FS_TELE_FEE_RATIO;
        public static int FS_TELE1_FEE;
        public static int FS_TELE2_FEE;
        public static TimeSpan FS_SUPPORT_FEE_RATIO;
        public static int FS_SUPPORT1_FEE;
        public static int FS_SUPPORT2_FEE;
        public static TimeSpan FS_MPREG_FEE_RATIO;
        public static int FS_MPREG1_FEE;
        public static int FS_MPREG2_FEE;
        public static TimeSpan FS_HPREG_FEE_RATIO;
        public static int FS_HPREG1_FEE;
        public static int FS_HPREG2_FEE;
        public static TimeSpan FS_EXPREG_FEE_RATIO;
        public static int FS_EXPREG1_FEE;
        public static int FS_EXPREG2_FEE;
        public static int FS_UPDATE_FRQ;
        public static int FS_BLOOD_OATH_COUNT;
        public static int FS_MAX_SUPPLY_LEVEL;
        public static int FS_FEE_FOR_CASTLE;
        public static int FS_MAX_OWN_TIME;

        // Feature Settings
        public static int TAKE_FORT_POINTS;
        public static int LOOSE_FORT_POINTS;
        public static int TAKE_CASTLE_POINTS;
        public static int LOOSE_CASTLE_POINTS;
        public static int CASTLE_DEFENDED_POINTS;
        public static int FESTIVAL_WIN_POINTS;
        public static int HERO_POINTS;
        public static int ROYAL_GUARD_COST;
        public static int KNIGHT_UNIT_COST;
        public static int KNIGHT_REINFORCE_COST;
        public static int BALLISTA_POINTS;
        public static int BLOODALLIANCE_POINTS;
        public static int BLOODOATH_POINTS;
        public static int KNIGHTSEPAULETTE_POINTS;
        public static int REPUTATION_SCORE_PER_KILL;
        public static int JOIN_ACADEMY_MIN_REP_SCORE;
        public static int JOIN_ACADEMY_MAX_REP_SCORE;
        public static int LVL_UP_20_AND_25_REP_SCORE;
        public static int LVL_UP_26_AND_30_REP_SCORE;
        public static int LVL_UP_31_AND_35_REP_SCORE;
        public static int LVL_UP_36_AND_40_REP_SCORE;
        public static int LVL_UP_41_AND_45_REP_SCORE;
        public static int LVL_UP_46_AND_50_REP_SCORE;
        public static int LVL_UP_51_AND_55_REP_SCORE;
        public static int LVL_UP_56_AND_60_REP_SCORE;
        public static int LVL_UP_61_AND_65_REP_SCORE;
        public static int LVL_UP_66_AND_70_REP_SCORE;
        public static int LVL_UP_71_AND_75_REP_SCORE;
        public static int LVL_UP_76_AND_80_REP_SCORE;
        public static int LVL_UP_81_AND_90_REP_SCORE;
        public static int LVL_UP_91_PLUS_REP_SCORE;
        public static double LVL_OBTAINED_REP_SCORE_MULTIPLIER;
        public static int CLAN_LEVEL_6_COST;
        public static int CLAN_LEVEL_7_COST;
        public static int CLAN_LEVEL_8_COST;
        public static int CLAN_LEVEL_9_COST;
        public static int CLAN_LEVEL_10_COST;
        public static int CLAN_LEVEL_6_REQUIREMENT;
        public static int CLAN_LEVEL_7_REQUIREMENT;
        public static int CLAN_LEVEL_8_REQUIREMENT;
        public static int CLAN_LEVEL_9_REQUIREMENT;
        public static int CLAN_LEVEL_10_REQUIREMENT;
        public static bool ALLOW_WYVERN_ALWAYS;
        public static bool ALLOW_WYVERN_DURING_SIEGE;
        public static bool ALLOW_MOUNTS_DURING_SIEGE;

        internal static void Load(string configPath)
        {
            ConfigurationParser parser = new(configPath);
            parser.LoadConfig(FileNames.Configs.FeatureConfigFile);

            // Castle Settings
            SIEGE_HOUR_LIST = parser.GetIntList("SiegeHourList");
            CASTLE_BUY_TAX_NEUTRAL = parser.getInt("BuyTaxForNeutralSide", 15);
            CASTLE_BUY_TAX_LIGHT = parser.getInt("BuyTaxForLightSide");
            CASTLE_BUY_TAX_DARK = parser.getInt("BuyTaxForDarkSide", 30);
            CASTLE_SELL_TAX_NEUTRAL = parser.getInt("SellTaxForNeutralSide");
            CASTLE_SELL_TAX_LIGHT = parser.getInt("SellTaxForLightSide");
            CASTLE_SELL_TAX_DARK = parser.getInt("SellTaxForDarkSide", 20);
            CS_TELE_FEE_RATIO = parser.getLong("CastleTeleportFunctionFeeRatio", 604800000);
            CS_TELE1_FEE = parser.getInt("CastleTeleportFunctionFeeLvl1", 1000);
            CS_TELE2_FEE = parser.getInt("CastleTeleportFunctionFeeLvl2", 10000);
            CS_SUPPORT_FEE_RATIO = parser.getLong("CastleSupportFunctionFeeRatio", 604800000);
            CS_SUPPORT1_FEE = parser.getInt("CastleSupportFeeLvl1", 49000);
            CS_SUPPORT2_FEE = parser.getInt("CastleSupportFeeLvl2", 120000);
            CS_MPREG_FEE_RATIO = parser.getLong("CastleMpRegenerationFunctionFeeRatio", 604800000);
            CS_MPREG1_FEE = parser.getInt("CastleMpRegenerationFeeLvl1", 45000);
            CS_MPREG2_FEE = parser.getInt("CastleMpRegenerationFeeLvl2", 65000);
            CS_HPREG_FEE_RATIO = parser.getLong("CastleHpRegenerationFunctionFeeRatio", 604800000);
            CS_HPREG1_FEE = parser.getInt("CastleHpRegenerationFeeLvl1", 12000);
            CS_HPREG2_FEE = parser.getInt("CastleHpRegenerationFeeLvl2", 20000);
            CS_EXPREG_FEE_RATIO = parser.getLong("CastleExpRegenerationFunctionFeeRatio", 604800000);
            CS_EXPREG1_FEE = parser.getInt("CastleExpRegenerationFeeLvl1", 63000);
            CS_EXPREG2_FEE = parser.getInt("CastleExpRegenerationFeeLvl2", 70000);
            OUTER_DOOR_UPGRADE_PRICE2 = parser.getInt("OuterDoorUpgradePriceLvl2", 3000000);
            OUTER_DOOR_UPGRADE_PRICE3 = parser.getInt("OuterDoorUpgradePriceLvl3", 4000000);
            OUTER_DOOR_UPGRADE_PRICE5 = parser.getInt("OuterDoorUpgradePriceLvl5", 5000000);
            INNER_DOOR_UPGRADE_PRICE2 = parser.getInt("InnerDoorUpgradePriceLvl2", 750000);
            INNER_DOOR_UPGRADE_PRICE3 = parser.getInt("InnerDoorUpgradePriceLvl3", 900000);
            INNER_DOOR_UPGRADE_PRICE5 = parser.getInt("InnerDoorUpgradePriceLvl5", 1000000);
            WALL_UPGRADE_PRICE2 = parser.getInt("WallUpgradePriceLvl2", 1600000);
            WALL_UPGRADE_PRICE3 = parser.getInt("WallUpgradePriceLvl3", 1800000);
            WALL_UPGRADE_PRICE5 = parser.getInt("WallUpgradePriceLvl5", 2000000);
            TRAP_UPGRADE_PRICE1 = parser.getInt("TrapUpgradePriceLvl1", 3000000);
            TRAP_UPGRADE_PRICE2 = parser.getInt("TrapUpgradePriceLvl2", 4000000);
            TRAP_UPGRADE_PRICE3 = parser.getInt("TrapUpgradePriceLvl3", 5000000);
            TRAP_UPGRADE_PRICE4 = parser.getInt("TrapUpgradePriceLvl4", 6000000);

            // Fortress Settings
            FS_TELE_FEE_RATIO =
                TimeSpan.FromMilliseconds(parser.getLong("FortressTeleportFunctionFeeRatio", 604800000));

            FS_TELE1_FEE = parser.getInt("FortressTeleportFunctionFeeLvl1", 1000);
            FS_TELE2_FEE = parser.getInt("FortressTeleportFunctionFeeLvl2", 10000);
            FS_SUPPORT_FEE_RATIO =
                TimeSpan.FromMilliseconds(parser.getLong("FortressSupportFunctionFeeRatio", 86400000));

            FS_SUPPORT1_FEE = parser.getInt("FortressSupportFeeLvl1", 7000);
            FS_SUPPORT2_FEE = parser.getInt("FortressSupportFeeLvl2", 17000);
            FS_MPREG_FEE_RATIO =
                TimeSpan.FromMilliseconds(parser.getLong("FortressMpRegenerationFunctionFeeRatio", 86400000));

            FS_MPREG1_FEE = parser.getInt("FortressMpRegenerationFeeLvl1", 6500);
            FS_MPREG2_FEE = parser.getInt("FortressMpRegenerationFeeLvl2", 9300);
            FS_HPREG_FEE_RATIO =
                TimeSpan.FromMilliseconds(parser.getLong("FortressHpRegenerationFunctionFeeRatio", 86400000));

            FS_HPREG1_FEE = parser.getInt("FortressHpRegenerationFeeLvl1", 2000);
            FS_HPREG2_FEE = parser.getInt("FortressHpRegenerationFeeLvl2", 3500);
            FS_EXPREG_FEE_RATIO =
                TimeSpan.FromMilliseconds(parser.getLong("FortressExpRegenerationFunctionFeeRatio", 86400000));

            FS_EXPREG1_FEE = parser.getInt("FortressExpRegenerationFeeLvl1", 9000);
            FS_EXPREG2_FEE = parser.getInt("FortressExpRegenerationFeeLvl2", 10000);
            FS_UPDATE_FRQ = parser.getInt("FortressPeriodicUpdateFrequency", 360);
            FS_BLOOD_OATH_COUNT = parser.getInt("FortressBloodOathCount", 1);
            FS_MAX_SUPPLY_LEVEL = parser.getInt("FortressMaxSupplyLevel", 6);
            FS_FEE_FOR_CASTLE = parser.getInt("FortressFeeForCastle", 25000);
            FS_MAX_OWN_TIME = parser.getInt("FortressMaximumOwnTime", 168);

            // Feature Settings
            TAKE_FORT_POINTS = parser.getInt("TakeFortPoints", 200);
            LOOSE_FORT_POINTS = parser.getInt("LooseFortPoints");
            TAKE_CASTLE_POINTS = parser.getInt("TakeCastlePoints", 1500);
            LOOSE_CASTLE_POINTS = parser.getInt("LooseCastlePoints", 3000);
            CASTLE_DEFENDED_POINTS = parser.getInt("CastleDefendedPoints", 750);
            FESTIVAL_WIN_POINTS = parser.getInt("FestivalOfDarknessWin", 200);
            HERO_POINTS = parser.getInt("HeroPoints", 1000);
            ROYAL_GUARD_COST = parser.getInt("CreateRoyalGuardCost", 5000);
            KNIGHT_UNIT_COST = parser.getInt("CreateKnightUnitCost", 10000);
            KNIGHT_REINFORCE_COST = parser.getInt("ReinforceKnightUnitCost", 5000);
            BALLISTA_POINTS = parser.getInt("KillBallistaPoints", 500);
            BLOODALLIANCE_POINTS = parser.getInt("BloodAlliancePoints", 500);
            BLOODOATH_POINTS = parser.getInt("BloodOathPoints", 200);
            KNIGHTSEPAULETTE_POINTS = parser.getInt("KnightsEpaulettePoints", 20);
            REPUTATION_SCORE_PER_KILL = parser.getInt("ReputationScorePerKill", 1);
            JOIN_ACADEMY_MIN_REP_SCORE = parser.getInt("CompleteAcademyMinPoints", 190);
            JOIN_ACADEMY_MAX_REP_SCORE = parser.getInt("CompleteAcademyMaxPoints", 650);
            LVL_UP_20_AND_25_REP_SCORE = parser.getInt("LevelUp20And25ReputationScore", 4);
            LVL_UP_26_AND_30_REP_SCORE = parser.getInt("LevelUp26And30ReputationScore", 8);
            LVL_UP_31_AND_35_REP_SCORE = parser.getInt("LevelUp31And35ReputationScore", 12);
            LVL_UP_36_AND_40_REP_SCORE = parser.getInt("LevelUp36And40ReputationScore", 16);
            LVL_UP_41_AND_45_REP_SCORE = parser.getInt("LevelUp41And45ReputationScore", 25);
            LVL_UP_46_AND_50_REP_SCORE = parser.getInt("LevelUp46And50ReputationScore", 30);
            LVL_UP_51_AND_55_REP_SCORE = parser.getInt("LevelUp51And55ReputationScore", 35);
            LVL_UP_56_AND_60_REP_SCORE = parser.getInt("LevelUp56And60ReputationScore", 40);
            LVL_UP_61_AND_65_REP_SCORE = parser.getInt("LevelUp61And65ReputationScore", 54);
            LVL_UP_66_AND_70_REP_SCORE = parser.getInt("LevelUp66And70ReputationScore", 63);
            LVL_UP_71_AND_75_REP_SCORE = parser.getInt("LevelUp71And75ReputationScore", 75);
            LVL_UP_76_AND_80_REP_SCORE = parser.getInt("LevelUp76And80ReputationScore", 90);
            LVL_UP_81_AND_90_REP_SCORE = parser.getInt("LevelUp81And90ReputationScore", 120);
            LVL_UP_91_PLUS_REP_SCORE = parser.getInt("LevelUp91PlusReputationScore", 150);
            LVL_OBTAINED_REP_SCORE_MULTIPLIER = parser.getDouble("LevelObtainedReputationScoreMultiplier", 1.0);
            CLAN_LEVEL_6_COST = parser.getInt("ClanLevel6Cost", 15000);
            CLAN_LEVEL_7_COST = parser.getInt("ClanLevel7Cost", 450000);
            CLAN_LEVEL_8_COST = parser.getInt("ClanLevel8Cost", 1000000);
            CLAN_LEVEL_9_COST = parser.getInt("ClanLevel9Cost", 2000000);
            CLAN_LEVEL_10_COST = parser.getInt("ClanLevel10Cost", 4000000);
            CLAN_LEVEL_6_REQUIREMENT = parser.getInt("ClanLevel6Requirement", 40);
            CLAN_LEVEL_7_REQUIREMENT = parser.getInt("ClanLevel7Requirement", 40);
            CLAN_LEVEL_8_REQUIREMENT = parser.getInt("ClanLevel8Requirement", 40);
            CLAN_LEVEL_9_REQUIREMENT = parser.getInt("ClanLevel9Requirement", 40);
            CLAN_LEVEL_10_REQUIREMENT = parser.getInt("ClanLevel10Requirement", 40);
            ALLOW_WYVERN_ALWAYS = parser.getBoolean("AllowRideWyvernAlways");
            ALLOW_WYVERN_DURING_SIEGE = parser.getBoolean("AllowRideWyvernDuringSiege", true);
            ALLOW_MOUNTS_DURING_SIEGE = parser.getBoolean("AllowRideMountsDuringSiege");
        }
    }
}