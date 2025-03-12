using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkill
{
    [XmlElement(XmlSkillParameterNames.AbnormalInstant, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.AbnormalLevel, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.AbnormalResists, typeof(string))]
    [XmlElement(XmlSkillParameterNames.AbnormalTime, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.AbnormalType, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.AbnormalVisualEffect, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.ActivateRate, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.AffectHeight, typeof(string))]
    [XmlElement(XmlSkillParameterNames.AffectLimit, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.AffectObject, typeof(string))]
    [XmlElement(XmlSkillParameterNames.AffectRange, typeof(XmlSkillValue<ushort>))]
    [XmlElement(XmlSkillParameterNames.AffectScope, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.AttachToggleGroupId, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.AttributeType, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.AttributeValue, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillParameterNames.BasicProperty, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.BlockActionUseSkill, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.BlockedInOlympiad, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.BuffProtectLevel, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.CanBeDispelled, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.CanCastWhileDisabled, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.CanDoubleCast, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.CastRange, typeof(XmlSkillValue<short>))]
    [XmlElement(XmlSkillParameterNames.ChannelingEffects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.ChannelingSkillId, typeof(ushort))]
    [XmlElement(XmlSkillParameterNames.ChannelingStart, typeof(decimal))]
    [XmlElement(XmlSkillParameterNames.ChannelingTickInterval, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.ChargeConsume, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillParameterNames.ClanRepConsume, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.Cond, typeof(XmlSkillCond))]
    [XmlElement(XmlSkillParameterNames.Conditions, typeof(XmlSkillConditionList))]
    [XmlElement(XmlSkillParameterNames.CoolTime, typeof(XmlSkillValue<ushort>))]
    [XmlElement(XmlSkillParameterNames.DeleteAbnormalOnLeave, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.DisplayInList, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.DoubleCastSkill, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.EffectPoint, typeof(XmlSkillValue<short>))]
    [XmlElement(XmlSkillParameterNames.EffectRange, typeof(XmlSkillValue<short>))]
    [XmlElement(XmlSkillParameterNames.Effects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.EndEffects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.ExcludedFromCheck, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.FamePointConsume, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.FanRange, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.HalfKillRate, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.HitCancelTime, typeof(decimal))]
    [XmlElement(XmlSkillParameterNames.HitTime, typeof(XmlSkillValue<ushort>))]
    [XmlElement(XmlSkillParameterNames.HpConsume, typeof(XmlSkillValue<short>))]
    [XmlElement(XmlSkillParameterNames.Icon, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.InheritElementals, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.InheritPercent, typeof(decimal))]
    [XmlElement(XmlSkillParameterNames.IrreplacableBuff, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsDebuff, typeof(XmlSkillValue<bool>))]
    [XmlElement(XmlSkillParameterNames.IsHidingMessages, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsMagic, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillParameterNames.IsMentoring, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsNecessaryToggle, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsOutpost, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsRecoveryHerb, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsSharedWithSummon, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsSuicideAttack, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.IsTriggeredSkill, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.ItemConsumeCount, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillParameterNames.ItemConsumeId, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.ItemConsumeSteps, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.LightSoulMaxConsume, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.LvlBonusRate, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillParameterNames.MagicCriticalRate, typeof(sbyte))]
    [XmlElement(XmlSkillParameterNames.MagicLevel, typeof(XmlSkillValue<sbyte>))]
    [XmlElement(XmlSkillParameterNames.MaxChance, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.MinChance, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.MinPledgeClass, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.MpConsume, typeof(XmlSkillValue<ushort>))]
    [XmlElement(XmlSkillParameterNames.MpInitialConsume, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillParameterNames.MpPerChanneling, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.NextAction, typeof(string))]
    [XmlElement(XmlSkillParameterNames.OperateType, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.PassiveConditions, typeof(XmlSkillConditionList))]
    [XmlElement(XmlSkillParameterNames.PveEffects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.PvpEffects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.RemovedOnAnyActionExceptMove, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.RemovedOnDamage, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.RemovedOnUnequipWeapon, typeof(bool))]
    [XmlElement(XmlSkillParameterNames.ReuseDelay, typeof(XmlSkillValue<uint>))]
    [XmlElement(XmlSkillParameterNames.ReuseDelayGroup, typeof(int))]
    [XmlElement(XmlSkillParameterNames.ReuseDelayType, typeof(string))]
    [XmlElement(XmlSkillParameterNames.SelfEffects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.ShadowSoulMaxConsume, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillParameterNames.SoulMaxConsumeCount, typeof(byte))]
    [XmlElement(XmlSkillParameterNames.SpecialLevel, typeof(XmlSkillValue<sbyte>))]
    [XmlElement(XmlSkillParameterNames.StartEffects, typeof(XmlSkillEffectList))]
    [XmlElement(XmlSkillParameterNames.StaticReuse, typeof(XmlSkillValue<bool>))]
    [XmlElement(XmlSkillParameterNames.StayAfterDeath, typeof(string))]
    [XmlElement(XmlSkillParameterNames.SubordinationAbnormalType, typeof(string))]
    [XmlElement(XmlSkillParameterNames.TargetConditions, typeof(XmlSkillConditionList))]
    [XmlElement(XmlSkillParameterNames.TargetType, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.ToggleGroupId, typeof(int))]
    [XmlElement(XmlSkillParameterNames.Trait, typeof(XmlSkillValue))]
    [XmlElement(XmlSkillParameterNames.Variable, typeof(XmlSkillVariable))]
    [XmlElement(XmlSkillParameterNames.WithoutAction, typeof(bool))]
    [XmlChoiceIdentifier(nameof(ParameterTypes))]
    public object[]? Parameters { get; set; }

    [XmlIgnore]
    public XmlSkillParameterType[]? ParameterTypes { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("toLevel")]
    public byte ToLevel { get; set; }

    [XmlIgnore]
    public bool ToLevelSpecified { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("displayId")]
    public int DisplayId { get; set; }

    [XmlIgnore]
    public bool DisplayIdSpecified { get; set; }

    [XmlAttribute("displayLevel")]
    public byte DisplayLevel { get; set; }

    [XmlIgnore]
    public bool DisplayLevelSpecified { get; set; }

    [XmlAttribute("referenceId")]
    public int ReferenceId { get; set; }

    [XmlIgnore]
    public bool ReferenceIdSpecified { get; set; }
}