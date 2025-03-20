namespace L2Dn.GameServer.Enums;

/// <summary>
/// Effect types.
/// </summary>
[Flags]
public enum EffectTypes: long
{
    NONE = 0,

    AGGRESSION = 1L << 0,
    CHARM_OF_LUCK = 1L << 1,
    CPHEAL = 1L << 2,
    DISPEL = 1L << 3,
    DISPEL_BY_SLOT = 1L << 4,
    DMG_OVER_TIME = 1L << 5,
    DMG_OVER_TIME_PERCENT = 1L << 6,
    MAGICAL_DMG_OVER_TIME = 1L << 7,
    DEATH_LINK = 1L << 8,
    BLOCK_CONTROL = 1L << 9,
    EXTRACT_ITEM = 1L << 10,
    FISHING = 1L << 11,
    FISHING_START = 1L << 12,
    HATE = 1L << 13,
    HEAL = 1L << 14,
    HP_DRAIN = 1L << 15,
    MAGICAL_ATTACK = 1L << 16,
    MANAHEAL_BY_LEVEL = 1L << 17,
    MANAHEAL_PERCENT = 1L << 18,
    MUTE = 1L << 19,
    NOBLESSE_BLESSING = 1L << 20,
    PHYSICAL_ATTACK = 1L << 21,
    PHYSICAL_ATTACK_HP_LINK = 1L << 22,
    LETHAL_ATTACK = 1L << 23,
    REGULAR_ATTACK = 1L << 24,
    REBALANCE_HP = 1L << 25,
    REFUEL_AIRSHIP = 1L << 26,
    RELAXING = 1L << 27,
    RESURRECTION = 1L << 28,
    RESURRECTION_SPECIAL = 1L << 29,
    ROOT = 1L << 30,
    SLEEP = 1L << 31,
    STEAL_ABNORMAL = 1L << 32,
    BLOCK_ACTIONS = 1L << 33,
    SUMMON = 1L << 34,
    SUMMON_PET = 1L << 35,
    SUMMON_NPC = 1L << 36,
    TELEPORT = 1L << 37,
    TELEPORT_TO_TARGET = 1L << 38,
    ABNORMAL_SHIELD = 1L << 39,
    DUAL_RANGE = 1L << 40,
    VITALITY_POINT_UP = 1L << 41,
}