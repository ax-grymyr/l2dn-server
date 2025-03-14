using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class NpcStatMultipliers
    {
        public static bool ENABLE_NPC_STAT_MULTIPLIERS;
        public static double MONSTER_HP_MULTIPLIER;
        public static double MONSTER_MP_MULTIPLIER;
        public static double MONSTER_PATK_MULTIPLIER;
        public static double MONSTER_MATK_MULTIPLIER;
        public static double MONSTER_PDEF_MULTIPLIER;
        public static double MONSTER_MDEF_MULTIPLIER;
        public static double MONSTER_AGRRO_RANGE_MULTIPLIER;
        public static double MONSTER_CLAN_HELP_RANGE_MULTIPLIER;
        public static double RAIDBOSS_HP_MULTIPLIER;
        public static double RAIDBOSS_MP_MULTIPLIER;
        public static double RAIDBOSS_PATK_MULTIPLIER;
        public static double RAIDBOSS_MATK_MULTIPLIER;
        public static double RAIDBOSS_PDEF_MULTIPLIER;
        public static double RAIDBOSS_MDEF_MULTIPLIER;
        public static double RAIDBOSS_AGRRO_RANGE_MULTIPLIER;
        public static double RAIDBOSS_CLAN_HELP_RANGE_MULTIPLIER;
        public static double GUARD_HP_MULTIPLIER;
        public static double GUARD_MP_MULTIPLIER;
        public static double GUARD_PATK_MULTIPLIER;
        public static double GUARD_MATK_MULTIPLIER;
        public static double GUARD_PDEF_MULTIPLIER;
        public static double GUARD_MDEF_MULTIPLIER;
        public static double GUARD_AGRRO_RANGE_MULTIPLIER;
        public static double GUARD_CLAN_HELP_RANGE_MULTIPLIER;
        public static double DEFENDER_HP_MULTIPLIER;
        public static double DEFENDER_MP_MULTIPLIER;
        public static double DEFENDER_PATK_MULTIPLIER;
        public static double DEFENDER_MATK_MULTIPLIER;
        public static double DEFENDER_PDEF_MULTIPLIER;
        public static double DEFENDER_MDEF_MULTIPLIER;
        public static double DEFENDER_AGRRO_RANGE_MULTIPLIER;
        public static double DEFENDER_CLAN_HELP_RANGE_MULTIPLIER;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.NpcStatMultipliers);

            ENABLE_NPC_STAT_MULTIPLIERS = parser.getBoolean("EnableNpcStatMultipliers");
            MONSTER_HP_MULTIPLIER = parser.getDouble("MonsterHP", 1.0);
            MONSTER_MP_MULTIPLIER = parser.getDouble("MonsterMP", 1.0);
            MONSTER_PATK_MULTIPLIER = parser.getDouble("MonsterPAtk", 1.0);
            MONSTER_MATK_MULTIPLIER = parser.getDouble("MonsterMAtk", 1.0);
            MONSTER_PDEF_MULTIPLIER = parser.getDouble("MonsterPDef", 1.0);
            MONSTER_MDEF_MULTIPLIER = parser.getDouble("MonsterMDef", 1.0);
            MONSTER_AGRRO_RANGE_MULTIPLIER = parser.getDouble("MonsterAggroRange", 1.0);
            MONSTER_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("MonsterClanHelpRange", 1.0);
            RAIDBOSS_HP_MULTIPLIER = parser.getDouble("RaidbossHP", 1.0);
            RAIDBOSS_MP_MULTIPLIER = parser.getDouble("RaidbossMP", 1.0);
            RAIDBOSS_PATK_MULTIPLIER = parser.getDouble("RaidbossPAtk", 1.0);
            RAIDBOSS_MATK_MULTIPLIER = parser.getDouble("RaidbossMAtk", 1.0);
            RAIDBOSS_PDEF_MULTIPLIER = parser.getDouble("RaidbossPDef", 1.0);
            RAIDBOSS_MDEF_MULTIPLIER = parser.getDouble("RaidbossMDef", 1.0);
            RAIDBOSS_AGRRO_RANGE_MULTIPLIER = parser.getDouble("RaidbossAggroRange", 1.0);
            RAIDBOSS_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("RaidbossClanHelpRange", 1.0);
            GUARD_HP_MULTIPLIER = parser.getDouble("GuardHP", 1.0);
            GUARD_MP_MULTIPLIER = parser.getDouble("GuardMP", 1.0);
            GUARD_PATK_MULTIPLIER = parser.getDouble("GuardPAtk", 1.0);
            GUARD_MATK_MULTIPLIER = parser.getDouble("GuardMAtk", 1.0);
            GUARD_PDEF_MULTIPLIER = parser.getDouble("GuardPDef", 1.0);
            GUARD_MDEF_MULTIPLIER = parser.getDouble("GuardMDef", 1.0);
            GUARD_AGRRO_RANGE_MULTIPLIER = parser.getDouble("GuardAggroRange", 1.0);
            GUARD_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("GuardClanHelpRange", 1.0);
            DEFENDER_HP_MULTIPLIER = parser.getDouble("DefenderHP", 1.0);
            DEFENDER_MP_MULTIPLIER = parser.getDouble("DefenderMP", 1.0);
            DEFENDER_PATK_MULTIPLIER = parser.getDouble("DefenderPAtk", 1.0);
            DEFENDER_MATK_MULTIPLIER = parser.getDouble("DefenderMAtk", 1.0);
            DEFENDER_PDEF_MULTIPLIER = parser.getDouble("DefenderPDef", 1.0);
            DEFENDER_MDEF_MULTIPLIER = parser.getDouble("DefenderMDef", 1.0);
            DEFENDER_AGRRO_RANGE_MULTIPLIER = parser.getDouble("DefenderAggroRange", 1.0);
            DEFENDER_CLAN_HELP_RANGE_MULTIPLIER = parser.getDouble("DefenderClanHelpRange", 1.0);
        }
    }
}