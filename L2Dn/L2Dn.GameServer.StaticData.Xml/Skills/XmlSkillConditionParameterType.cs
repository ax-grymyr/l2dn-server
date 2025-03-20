using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public enum XmlSkillConditionParameterType
{
    [XmlEnum(XmlSkillConditionParameterNames.AffectType)]
    AffectType,

    [XmlEnum(XmlSkillConditionParameterNames.Alignment)]
    Alignment,

    [XmlEnum(XmlSkillConditionParameterNames.Amount)]
    Amount,

    [XmlEnum(XmlSkillConditionParameterNames.ArmorType)]
    ArmorType,

    [XmlEnum(XmlSkillConditionParameterNames.ClassIds)]
    ClassIds,

    [XmlEnum(XmlSkillConditionParameterNames.Distance)]
    Distance,

    [XmlEnum(XmlSkillConditionParameterNames.Direction)]
    Direction,

    [XmlEnum(XmlSkillConditionParameterNames.DoorIds)]
    DoorIds,

    [XmlEnum(XmlSkillConditionParameterNames.HasAbnormal)]
    HasAbnormal,

    [XmlEnum(XmlSkillConditionParameterNames.HasLearned)]
    HasLearned,

    [XmlEnum(XmlSkillConditionParameterNames.IncludeMe)]
    IncludeMe,

    [XmlEnum(XmlSkillConditionParameterNames.IsAround)]
    IsAround,

    [XmlEnum(XmlSkillConditionParameterNames.IsFemale)]
    IsFemale,

    [XmlEnum(XmlSkillConditionParameterNames.IsWithin)]
    IsWithin,

    [XmlEnum(XmlSkillConditionParameterNames.Less)]
    Less,

    [XmlEnum(XmlSkillConditionParameterNames.Level)]
    Level,

    [XmlEnum(XmlSkillConditionParameterNames.MaxLevel)]
    MaxLevel,

    [XmlEnum(XmlSkillConditionParameterNames.MinLevel)]
    MinLevel,

    [XmlEnum(XmlSkillConditionParameterNames.NpcId)]
    NpcId,

    [XmlEnum(XmlSkillConditionParameterNames.NpcIds)]
    NpcIds,

    [XmlEnum(XmlSkillConditionParameterNames.PercentType)]
    PercentType,

    [XmlEnum(XmlSkillConditionParameterNames.Race)]
    Race,

    [XmlEnum(XmlSkillConditionParameterNames.Range)]
    Range,

    [XmlEnum(XmlSkillConditionParameterNames.ResidenceIds)]
    ResidenceIds,

    [XmlEnum(XmlSkillConditionParameterNames.SkillId)]
    SkillId,

    [XmlEnum(XmlSkillConditionParameterNames.SkillIds)]
    SkillIds,

    [XmlEnum(XmlSkillConditionParameterNames.SkillLevel)]
    SkillLevel,

    [XmlEnum(XmlSkillConditionParameterNames.SlotsPercent)]
    SlotsPercent,

    [XmlEnum(XmlSkillConditionParameterNames.TeleportBookmarkSlots)]
    TeleportBookmarkSlots,

    [XmlEnum(XmlSkillConditionParameterNames.TransformId)]
    TransformId,

    [XmlEnum(XmlSkillConditionParameterNames.Type)]
    Type,

    [XmlEnum(XmlSkillConditionParameterNames.WeaponType)]
    WeaponType,

    [XmlEnum(XmlSkillConditionParameterNames.WeightPercent)]
    WeightPercent,
}