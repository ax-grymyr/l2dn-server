namespace L2Dn.GameServer.Enums;

/// <summary>
/// This enum holds the skill operative types.
/// </summary>
public enum SkillOperateType
{
    /// <summary>
    /// Active Skill with "Instant Effect" (for example damage skills heal/pdam/mdam/cpdam skills).
    /// </summary>
    A1,

    /// <summary>
    /// Active Skill with "Continuous effect + Instant effect" (for example buff/debuff or damage/heal over time skills).
    /// </summary>
    A2,

    /// <summary>
    /// Active Skill with "Continuous effect + Instant effect" (for example buff/debuff or damage/heal over time skills).
    /// </summary>
    A3,

    /// <summary>
    /// Active Skill with "Instant effect + ?" used for special event herb.
    /// </summary>
    A4,

    /// <summary>
    /// Aura Active Skill.
    /// </summary>
    A5,

    /// <summary>
    /// Synergy Active Skill.
    /// </summary>
    A6,

    /// <summary>
    /// Continuous Active Skill with "instant effect" (instant effect casted by ticks).
    /// </summary>
    CA1,

    /// <summary>
    /// ?
    /// </summary>
    CA2,

    /// <summary>
    /// Continuous Active Skill with "continuous effect" (continuous effect casted by ticks).
    /// </summary>
    CA5,

    /// <summary>
    /// Directional Active Skill with "Charge/Rush instant effect".
    /// </summary>
    DA1,

    /// <summary>
    /// Directional Active Skill with "Charge/Rush Continuous effect".
    /// </summary>
    DA2,

    /// <summary>
    /// Directional Active Skill with Blink effect.
    /// </summary>
    DA3,

    /// <summary>
    /// Directional Active Skill with "Left Continuous effect".
    /// </summary>
    DA4,

    /// <summary>
    /// Directional Active Skill with "Right Continuous effect".
    /// </summary>
    DA5,

    /// <summary>
    /// Passive Skill.
    /// </summary>
    P,

    /// <summary>
    /// Toggle Skill.
    /// </summary>
    T,

    /// <summary>
    /// Toggle Skill with Group.
    /// </summary>
    TG,

    /// <summary>
    /// Aura Skill.
    /// </summary>
    AU
}

public static class SkillOperateTypeUtil
{
    /// <summary>
    /// Verifies if the operative type correspond to an active skill.
    /// </summary>
    public static bool IsActive(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.A1 or SkillOperateType.A2 or SkillOperateType.A3 or SkillOperateType.A4
            or SkillOperateType.A5 or SkillOperateType.A6 or SkillOperateType.CA1 or SkillOperateType.CA5
            or SkillOperateType.DA1 or SkillOperateType.DA2 or SkillOperateType.DA4 or SkillOperateType.DA5;

    /// <summary>
    /// Verifies if the operative type correspond to a continuous skill.
    /// </summary>
    public static bool IsContinuous(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.A2 or SkillOperateType.A3 or SkillOperateType.A4 or SkillOperateType.A5
            or SkillOperateType.A6 or SkillOperateType.DA2 or SkillOperateType.DA4 or SkillOperateType.DA5;

    /// <summary>
    /// Verifies if the operative type correspond to a self continuous skill.
    /// </summary>
    public static bool IsSelfContinuous(this SkillOperateType skillOperateType) =>
        skillOperateType == SkillOperateType.A3;

    /// <summary>
    /// Verifies if the operative type correspond to a passive skill.
    /// </summary>
    public static bool IsPassive(this SkillOperateType skillOperateType) => skillOperateType == SkillOperateType.P;

    /// <summary>
    /// Verifies if the operative type correspond to a toggle skill.
    /// </summary>
    public static bool IsToggle(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.T or SkillOperateType.TG or SkillOperateType.AU;

    /// <summary>
    /// Verifies if the operative type correspond to a active aura skill.
    /// </summary>
    public static bool IsAura(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.A5 or SkillOperateType.A6 or SkillOperateType.AU;

    /// <summary>
    /// Verifies if the operate type skill type should not send messages for start/finish.
    /// </summary>
    public static bool IsHidingMessages(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.P or SkillOperateType.A5 or SkillOperateType.A6 or SkillOperateType.TG;

    /// <summary>
    /// Verifies if the operate type skill type should not be broadcasted as MagicSkillUse, MagicSkillLaunched.
    /// </summary>
    public static bool IsNotBroadcastable(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.AU or SkillOperateType.A5 or SkillOperateType.A6 or SkillOperateType.TG
            or SkillOperateType.T;

    /// <summary>
    /// Verifies if the operative type correspond to a channeling skill.
    /// </summary>
    public static bool IsChanneling(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.CA1 or SkillOperateType.CA2 or SkillOperateType.CA5;

    /// <summary>
    /// Verifies if the operative type correspond to a synergy skill.
    /// </summary>
    public static bool IsSynergy(this SkillOperateType skillOperateType) => skillOperateType == SkillOperateType.A6;

    /// <summary>
    /// Verifies if the operative type correspond to a fly type skill.
    /// </summary>
    public static bool IsFlyType(this SkillOperateType skillOperateType) =>
        skillOperateType is SkillOperateType.DA1 or SkillOperateType.DA2 or SkillOperateType.DA3 or SkillOperateType.DA4
            or SkillOperateType.DA5;
}