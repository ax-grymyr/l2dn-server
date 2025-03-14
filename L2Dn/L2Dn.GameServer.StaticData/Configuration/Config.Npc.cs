using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class Npc
    {
        public static bool ANNOUNCE_MAMMON_SPAWN;
        public static bool ALT_MOB_AGRO_IN_PEACEZONE;
        public static bool ALT_ATTACKABLE_NPCS;
        public static bool ALT_GAME_VIEWNPC;
        public static bool SHOW_NPC_LEVEL;
        public static bool SHOW_NPC_AGGRESSION;
        public static bool ATTACKABLES_CAMP_PLAYER_CORPSES;
        public static bool SHOW_CREST_WITHOUT_QUEST;
        public static bool ENABLE_RANDOM_ENCHANT_EFFECT;
        public static int MIN_NPC_LEVEL_DMG_PENALTY;
        public static ImmutableArray<double> NPC_DMG_PENALTY = ImmutableArray<double>.Empty;
        public static ImmutableArray<double> NPC_CRIT_DMG_PENALTY = ImmutableArray<double>.Empty;
        public static ImmutableArray<double> NPC_SKILL_DMG_PENALTY = ImmutableArray<double>.Empty;
        public static int MIN_NPC_LEVEL_MAGIC_PENALTY;
        public static ImmutableArray<double> NPC_SKILL_CHANCE_PENALTY = ImmutableArray<double>.Empty;
        public static int DEFAULT_CORPSE_TIME;
        public static int SPOILED_CORPSE_EXTEND_TIME;
        public static TimeSpan CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY;
        public static int MAX_DRIFT_RANGE;
        public static bool AGGRO_DISTANCE_CHECK_ENABLED;
        public static int AGGRO_DISTANCE_CHECK_RANGE;
        public static bool AGGRO_DISTANCE_CHECK_RAIDS;
        public static int AGGRO_DISTANCE_CHECK_RAID_RANGE;
        public static bool AGGRO_DISTANCE_CHECK_INSTANCES;
        public static bool AGGRO_DISTANCE_CHECK_RESTORE_LIFE;
        public static bool GUARD_ATTACK_AGGRO_MOB;
        public static double RAID_HP_REGEN_MULTIPLIER;
        public static double RAID_MP_REGEN_MULTIPLIER;
        public static double RAID_PDEFENCE_MULTIPLIER;
        public static double RAID_MDEFENCE_MULTIPLIER;
        public static double RAID_PATTACK_MULTIPLIER;
        public static double RAID_MATTACK_MULTIPLIER;
        public static double RAID_MINION_RESPAWN_TIMER;
        public static ImmutableDictionary<int, int> MINIONS_RESPAWN_TIME = ImmutableDictionary<int, int>.Empty;
        public static float RAID_MIN_RESPAWN_MULTIPLIER;
        public static float RAID_MAX_RESPAWN_MULTIPLIER;
        public static bool RAID_DISABLE_CURSE;
        public static bool FORCE_DELETE_MINIONS;
        public static int RAID_CHAOS_TIME;
        public static int GRAND_CHAOS_TIME;
        public static int MINION_CHAOS_TIME;
        public static int INVENTORY_MAXIMUM_PET;
        public static double PET_HP_REGEN_MULTIPLIER;
        public static double PET_MP_REGEN_MULTIPLIER;
        public static int VITALITY_CONSUME_BY_MOB;
        public static int VITALITY_CONSUME_BY_BOSS;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.Npc);

            ANNOUNCE_MAMMON_SPAWN = parser.getBoolean("AnnounceMammonSpawn");
            ALT_MOB_AGRO_IN_PEACEZONE = parser.getBoolean("AltMobAgroInPeaceZone", true);
            ALT_ATTACKABLE_NPCS = parser.getBoolean("AltAttackableNpcs", true);
            ALT_GAME_VIEWNPC = parser.getBoolean("AltGameViewNpc");
            SHOW_NPC_LEVEL = parser.getBoolean("ShowNpcLevel");
            SHOW_NPC_AGGRESSION = parser.getBoolean("ShowNpcAggression");
            ATTACKABLES_CAMP_PLAYER_CORPSES = parser.getBoolean("AttackablesCampPlayerCorpses");
            SHOW_CREST_WITHOUT_QUEST = parser.getBoolean("ShowCrestWithoutQuest");
            ENABLE_RANDOM_ENCHANT_EFFECT = parser.getBoolean("EnableRandomEnchantEffect");
            MIN_NPC_LEVEL_DMG_PENALTY = parser.getInt("MinNPCLevelForDmgPenalty", 78);
            NPC_DMG_PENALTY = parser.GetDoubleList("DmgPenaltyForLvLDifferences", ',', 0.7, 0.6, 0.6, 0.55);
            NPC_CRIT_DMG_PENALTY = parser.GetDoubleList("CritDmgPenaltyForLvLDifferences", ',', 0.75, 0.65, 0.6, 0.58);
            NPC_SKILL_DMG_PENALTY = parser.GetDoubleList("SkillDmgPenaltyForLvLDifferences", ',', 0.8, 0.7, 0.65, 0.62);
            MIN_NPC_LEVEL_MAGIC_PENALTY = parser.getInt("MinNPCLevelForMagicPenalty", 78);
            NPC_SKILL_CHANCE_PENALTY =
                parser.GetDoubleList("SkillChancePenaltyForLvLDifferences", ',', 2.5, 3.0, 3.25, 3.5);

            DEFAULT_CORPSE_TIME = parser.getInt("DefaultCorpseTime", 7);
            SPOILED_CORPSE_EXTEND_TIME = parser.getInt("SpoiledCorpseExtendTime", 10);
            CORPSE_CONSUME_SKILL_ALLOWED_TIME_BEFORE_DECAY = TimeSpan.FromMilliseconds(
                parser.getInt("CorpseConsumeSkillAllowedTimeBeforeDecay", 2000));

            MAX_DRIFT_RANGE = parser.getInt("MaxDriftRange", 300);
            AGGRO_DISTANCE_CHECK_ENABLED = parser.getBoolean("AggroDistanceCheckEnabled", true);
            AGGRO_DISTANCE_CHECK_RANGE = parser.getInt("AggroDistanceCheckRange", 1500);
            AGGRO_DISTANCE_CHECK_RAIDS = parser.getBoolean("AggroDistanceCheckRaids");
            AGGRO_DISTANCE_CHECK_RAID_RANGE = parser.getInt("AggroDistanceCheckRaidRange", 3000);
            AGGRO_DISTANCE_CHECK_INSTANCES = parser.getBoolean("AggroDistanceCheckInstances");
            AGGRO_DISTANCE_CHECK_RESTORE_LIFE = parser.getBoolean("AggroDistanceCheckRestoreLife", true);
            GUARD_ATTACK_AGGRO_MOB = parser.getBoolean("GuardAttackAggroMob");
            RAID_HP_REGEN_MULTIPLIER = parser.getDouble("RaidHpRegenMultiplier", 100.0) / 100.0;
            RAID_MP_REGEN_MULTIPLIER = parser.getDouble("RaidMpRegenMultiplier", 100.0) / 100.0;
            RAID_PDEFENCE_MULTIPLIER = parser.getDouble("RaidPDefenceMultiplier", 100.0) / 100.0;
            RAID_MDEFENCE_MULTIPLIER = parser.getDouble("RaidMDefenceMultiplier", 100.0) / 100.0;
            RAID_PATTACK_MULTIPLIER = parser.getDouble("RaidPAttackMultiplier", 100.0) / 100.0;
            RAID_MATTACK_MULTIPLIER = parser.getDouble("RaidMAttackMultiplier", 100.0) / 100.0;
            RAID_MIN_RESPAWN_MULTIPLIER = parser.getFloat("RaidMinRespawnMultiplier", 1.0f);
            RAID_MAX_RESPAWN_MULTIPLIER = parser.getFloat("RaidMaxRespawnMultiplier", 1.0f);
            RAID_MINION_RESPAWN_TIMER = parser.getInt("RaidMinionRespawnTime", 300000);
            MINIONS_RESPAWN_TIME = parser.GetIdValueMap<int>("CustomMinionsRespawnTime");
            FORCE_DELETE_MINIONS = parser.getBoolean("ForceDeleteMinions");
            RAID_DISABLE_CURSE = parser.getBoolean("DisableRaidCurse");
            RAID_CHAOS_TIME = parser.getInt("RaidChaosTime", 10);
            GRAND_CHAOS_TIME = parser.getInt("GrandChaosTime", 10);
            MINION_CHAOS_TIME = parser.getInt("MinionChaosTime", 10);
            INVENTORY_MAXIMUM_PET = parser.getInt("MaximumSlotsForPet", 12);
            PET_HP_REGEN_MULTIPLIER = parser.getDouble("PetHpRegenMultiplier", 100) / 100;
            PET_MP_REGEN_MULTIPLIER = parser.getDouble("PetMpRegenMultiplier", 100) / 100;
            VITALITY_CONSUME_BY_MOB = parser.getInt("VitalityConsumeByMob", 2250);
            VITALITY_CONSUME_BY_BOSS = parser.getInt("VitalityConsumeByBoss", 1125);
        }
    }
}