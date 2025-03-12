using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public enum XmlSkillParameterType
{
    [XmlEnum(XmlSkillParameterNames.AbnormalInstant)]
    AbnormalInstant,

    [XmlEnum(XmlSkillParameterNames.AbnormalLevel)]
    AbnormalLevel,

    [XmlEnum(XmlSkillParameterNames.AbnormalResists)]
    AbnormalResists,

    [XmlEnum(XmlSkillParameterNames.AbnormalTime)]
    AbnormalTime,

    [XmlEnum(XmlSkillParameterNames.AbnormalType)]
    AbnormalType,

    [XmlEnum(XmlSkillParameterNames.AbnormalVisualEffect)]
    AbnormalVisualEffect,

    [XmlEnum(XmlSkillParameterNames.ActivateRate)]
    ActivateRate,

    [XmlEnum(XmlSkillParameterNames.AffectHeight)]
    AffectHeight,

    [XmlEnum(XmlSkillParameterNames.AffectLimit)]
    AffectLimit,

    [XmlEnum(XmlSkillParameterNames.AffectObject)]
    AffectObject,

    [XmlEnum(XmlSkillParameterNames.AffectRange)]
    AffectRange,

    [XmlEnum(XmlSkillParameterNames.AffectScope)]
    AffectScope,

    [XmlEnum(XmlSkillParameterNames.AttachToggleGroupId)]
    AttachToggleGroupId,

    [XmlEnum(XmlSkillParameterNames.AttributeType)]
    AttributeType,

    [XmlEnum(XmlSkillParameterNames.AttributeValue)]
    AttributeValue,

    [XmlEnum(XmlSkillParameterNames.BasicProperty)]
    BasicProperty,

    [XmlEnum(XmlSkillParameterNames.BlockActionUseSkill)]
    BlockActionUseSkill,

    [XmlEnum(XmlSkillParameterNames.BlockedInOlympiad)]
    BlockedInOlympiad,

    [XmlEnum(XmlSkillParameterNames.BuffProtectLevel)]
    BuffProtectLevel,

    [XmlEnum(XmlSkillParameterNames.CanBeDispelled)]
    CanBeDispelled,

    [XmlEnum(XmlSkillParameterNames.CanCastWhileDisabled)]
    CanCastWhileDisabled,

    [XmlEnum(XmlSkillParameterNames.CanDoubleCast)]
    CanDoubleCast,

    [XmlEnum(XmlSkillParameterNames.CastRange)]
    CastRange,

    [XmlEnum(XmlSkillParameterNames.ChannelingEffects)]
    ChannelingEffects,

    [XmlEnum(XmlSkillParameterNames.ChannelingSkillId)]
    ChannelingSkillId,

    [XmlEnum(XmlSkillParameterNames.ChannelingStart)]
    ChannelingStart,

    [XmlEnum(XmlSkillParameterNames.ChannelingTickInterval)]
    ChannelingTickInterval,

    [XmlEnum(XmlSkillParameterNames.ChargeConsume)]
    ChargeConsume,

    [XmlEnum(XmlSkillParameterNames.ClanRepConsume)]
    ClanRepConsume,

    [XmlEnum(XmlSkillParameterNames.Cond)]
    Cond,

    [XmlEnum(XmlSkillParameterNames.Conditions)]
    Conditions,

    [XmlEnum(XmlSkillParameterNames.CoolTime)]
    CoolTime,

    [XmlEnum(XmlSkillParameterNames.DeleteAbnormalOnLeave)]
    DeleteAbnormalOnLeave,

    [XmlEnum(XmlSkillParameterNames.DisplayInList)]
    DisplayInList,

    [XmlEnum(XmlSkillParameterNames.DoubleCastSkill)]
    DoubleCastSkill,

    [XmlEnum(XmlSkillParameterNames.EffectPoint)]
    EffectPoint,

    [XmlEnum(XmlSkillParameterNames.EffectRange)]
    EffectRange,

    [XmlEnum(XmlSkillParameterNames.Effects)]
    Effects,

    [XmlEnum(XmlSkillParameterNames.EndEffects)]
    EndEffects,

    [XmlEnum(XmlSkillParameterNames.ExcludedFromCheck)]
    ExcludedFromCheck,

    [XmlEnum(XmlSkillParameterNames.FamePointConsume)]
    FamePointConsume,

    [XmlEnum(XmlSkillParameterNames.FanRange)]
    FanRange,

    [XmlEnum(XmlSkillParameterNames.HalfKillRate)]
    HalfKillRate,

    [XmlEnum(XmlSkillParameterNames.HitCancelTime)]
    HitCancelTime,

    [XmlEnum(XmlSkillParameterNames.HitTime)]
    HitTime,

    [XmlEnum(XmlSkillParameterNames.HpConsume)]
    HpConsume,

    [XmlEnum(XmlSkillParameterNames.Icon)]
    Icon,

    [XmlEnum(XmlSkillParameterNames.InheritElementals)]
    InheritElementals,

    [XmlEnum(XmlSkillParameterNames.InheritPercent)]
    InheritPercent,

    [XmlEnum(XmlSkillParameterNames.IrreplacableBuff)]
    IrreplacableBuff,

    [XmlEnum(XmlSkillParameterNames.IsDebuff)]
    IsDebuff,

    [XmlEnum(XmlSkillParameterNames.IsHidingMessages)]
    IsHidingMessages,

    [XmlEnum(XmlSkillParameterNames.IsMagic)]
    IsMagic,

    [XmlEnum(XmlSkillParameterNames.IsMentoring)]
    IsMentoring,

    [XmlEnum(XmlSkillParameterNames.IsNecessaryToggle)]
    IsNecessaryToggle,

    [XmlEnum(XmlSkillParameterNames.IsOutpost)]
    IsOutpost,

    [XmlEnum(XmlSkillParameterNames.IsRecoveryHerb)]
    IsRecoveryHerb,

    [XmlEnum(XmlSkillParameterNames.IsSharedWithSummon)]
    IsSharedWithSummon,

    [XmlEnum(XmlSkillParameterNames.IsSuicideAttack)]
    IsSuicideAttack,

    [XmlEnum(XmlSkillParameterNames.IsTriggeredSkill)]
    IsTriggeredSkill,

    [XmlEnum(XmlSkillParameterNames.ItemConsumeCount)]
    ItemConsumeCount,

    [XmlEnum(XmlSkillParameterNames.ItemConsumeId)]
    ItemConsumeId,

    [XmlEnum(XmlSkillParameterNames.ItemConsumeSteps)]
    ItemConsumeSteps,

    [XmlEnum(XmlSkillParameterNames.LightSoulMaxConsume)]
    LightSoulMaxConsume,

    [XmlEnum(XmlSkillParameterNames.LvlBonusRate)]
    LvlBonusRate,

    [XmlEnum(XmlSkillParameterNames.MagicCriticalRate)]
    MagicCriticalRate,

    [XmlEnum(XmlSkillParameterNames.MagicLevel)]
    MagicLevel,

    [XmlEnum(XmlSkillParameterNames.MaxChance)]
    MaxChance,

    [XmlEnum(XmlSkillParameterNames.MinChance)]
    MinChance,

    [XmlEnum(XmlSkillParameterNames.MinPledgeClass)]
    MinPledgeClass,

    [XmlEnum(XmlSkillParameterNames.MpConsume)]
    MpConsume,

    [XmlEnum(XmlSkillParameterNames.MpInitialConsume)]
    MpInitialConsume,

    [XmlEnum(XmlSkillParameterNames.MpPerChanneling)]
    MpPerChanneling,

    [XmlEnum(XmlSkillParameterNames.NextAction)]
    NextAction,

    [XmlEnum(XmlSkillParameterNames.OperateType)]
    OperateType,

    [XmlEnum(XmlSkillParameterNames.PassiveConditions)]
    PassiveConditions,

    [XmlEnum(XmlSkillParameterNames.PveEffects)]
    PveEffects,

    [XmlEnum(XmlSkillParameterNames.PvpEffects)]
    PvpEffects,

    [XmlEnum(XmlSkillParameterNames.RemovedOnAnyActionExceptMove)]
    RemovedOnAnyActionExceptMove,

    [XmlEnum(XmlSkillParameterNames.RemovedOnDamage)]
    RemovedOnDamage,

    [XmlEnum(XmlSkillParameterNames.RemovedOnUnequipWeapon)]
    RemovedOnUnequipWeapon,

    [XmlEnum(XmlSkillParameterNames.ReuseDelay)]
    ReuseDelay,

    [XmlEnum(XmlSkillParameterNames.ReuseDelayGroup)]
    ReuseDelayGroup,

    [XmlEnum(XmlSkillParameterNames.ReuseDelayType)]
    ReuseDelayType,

    [XmlEnum(XmlSkillParameterNames.SelfEffects)]
    SelfEffects,

    [XmlEnum(XmlSkillParameterNames.ShadowSoulMaxConsume)]
    ShadowSoulMaxConsume,

    [XmlEnum(XmlSkillParameterNames.SoulMaxConsumeCount)]
    SoulMaxConsumeCount,

    [XmlEnum(XmlSkillParameterNames.SpecialLevel)]
    SpecialLevel,

    [XmlEnum(XmlSkillParameterNames.StartEffects)]
    StartEffects,

    [XmlEnum(XmlSkillParameterNames.StaticReuse)]
    StaticReuse,

    [XmlEnum(XmlSkillParameterNames.StayAfterDeath)]
    StayAfterDeath,

    [XmlEnum(XmlSkillParameterNames.SubordinationAbnormalType)]
    SubordinationAbnormalType,

    [XmlEnum(XmlSkillParameterNames.TargetConditions)]
    TargetConditions,

    [XmlEnum(XmlSkillParameterNames.TargetType)]
    TargetType,

    [XmlEnum(XmlSkillParameterNames.ToggleGroupId)]
    ToggleGroupId,

    [XmlEnum(XmlSkillParameterNames.Trait)]
    Trait,

    [XmlEnum(XmlSkillParameterNames.Variable)]
    Variable,

    [XmlEnum(XmlSkillParameterNames.WithoutAction)]
    WithoutAction,
}