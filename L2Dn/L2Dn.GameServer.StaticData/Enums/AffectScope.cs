namespace L2Dn.GameServer.Enums;

/// <summary>
/// Skill affect scope.
/// </summary>
public enum AffectScope
{
    /// <summary>
    /// Affects Valakas.
    /// </summary>
    BALAKAS_SCOPE,

    /// <summary>
    /// Affects dead clan mates.
    /// </summary>
    DEAD_PLEDGE,

    /// <summary>
    /// Affects dead union (Command Channel?) members.
    /// </summary>
    DEAD_UNION,

    /// <summary>
    /// Affects fan area.
    /// </summary>
    FAN,

    /// <summary>
    /// Affects fan area, using caster as point of origin.
    /// </summary>
    FAN_PB,

    /// <summary>
    /// Affects nothing.
    /// </summary>
    NONE,

    /// <summary>
    /// Affects party members.
    /// </summary>
    PARTY,

    /// <summary>
    /// Affects dead party members.
    /// </summary>
    DEAD_PARTY,

    /// <summary>
    /// Affects party and clan mates.
    /// </summary>
    PARTY_PLEDGE,

    /// <summary>
    /// Affects dead party and clan members.
    /// </summary>
    DEAD_PARTY_PLEDGE,

    /// <summary>
    /// Affects clan mates.
    /// </summary>
    PLEDGE,

    /// <summary>
    /// Affects point blank targets, using caster as point of origin.
    /// </summary>
    POINT_BLANK,

    /// <summary>
    /// Affects ranged targets, using selected target as point of origin.
    /// </summary>
    RANGE,

    /// <summary>
    /// Affects ranged targets, using selected target as point of origin sorted by lowest to highest HP.
    /// </summary>
    RANGE_SORT_BY_HP,

    /// <summary>
    /// Affects targets in donut shaped area, using caster as point of origin.
    /// </summary>
    RING_RANGE,

    /// <summary>
    /// Affects a single target.
    /// </summary>
    SINGLE,

    /// <summary>
    /// Affects targets inside an square area, using selected target as point of origin.
    /// </summary>
    SQUARE,

    /// <summary>
    /// Affects targets inside a square area, using caster as point of origin.
    /// </summary>
    SQUARE_PB,

    /// <summary>
    /// Affects static object targets.
    /// </summary>
    STATIC_OBJECT_SCOPE,

    /// <summary>
    /// Affects all summons except master.
    /// </summary>
    SUMMON_EXCEPT_MASTER,

    /// <summary>
    /// Affects wyverns.
    /// </summary>
    WYVERN_SCOPE
}