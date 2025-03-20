using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillCondition: XmlSkillLevelRestriction
{
    [XmlElement(XmlSkillConditionParameterNames.AffectType, typeof(string))]
    [XmlElement(XmlSkillConditionParameterNames.Alignment, typeof(string))]
    [XmlElement(XmlSkillConditionParameterNames.Amount, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillConditionParameterNames.ArmorType, typeof(XmlSkillStringList))]
    [XmlElement(XmlSkillConditionParameterNames.ClassIds, typeof(XmlSkillStringList))]
    [XmlElement(XmlSkillConditionParameterNames.Distance, typeof(XmlSkillValue<int>))]
    [XmlElement(XmlSkillConditionParameterNames.Direction, typeof(string))]
    [XmlElement(XmlSkillConditionParameterNames.DoorIds, typeof(XmlSkillIntList))]
    [XmlElement(XmlSkillConditionParameterNames.HasAbnormal, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.HasLearned, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.IncludeMe, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.IsAround, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.IsFemale, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.IsWithin, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.Less, typeof(bool))]
    [XmlElement(XmlSkillConditionParameterNames.Level, typeof(byte))]
    [XmlElement(XmlSkillConditionParameterNames.MaxLevel, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillConditionParameterNames.MinLevel, typeof(XmlSkillValue<byte>))]
    [XmlElement(XmlSkillConditionParameterNames.NpcId, typeof(int))]
    [XmlElement(XmlSkillConditionParameterNames.NpcIds, typeof(XmlSkillIntList))]
    [XmlElement(XmlSkillConditionParameterNames.PercentType, typeof(string))]
    [XmlElement(XmlSkillConditionParameterNames.Race, typeof(string))]
    [XmlElement(XmlSkillConditionParameterNames.Range, typeof(int))]
    [XmlElement(XmlSkillConditionParameterNames.ResidenceIds, typeof(XmlSkillIntList))]
    [XmlElement(XmlSkillConditionParameterNames.SkillId, typeof(int))]
    [XmlElement(XmlSkillConditionParameterNames.SkillIds, typeof(XmlSkillIntList))]
    [XmlElement(XmlSkillConditionParameterNames.SkillLevel, typeof(byte))]
    [XmlElement(XmlSkillConditionParameterNames.SlotsPercent, typeof(byte))]
    [XmlElement(XmlSkillConditionParameterNames.TeleportBookmarkSlots, typeof(byte))]
    [XmlElement(XmlSkillConditionParameterNames.TransformId, typeof(int))]
    [XmlElement(XmlSkillConditionParameterNames.Type, typeof(string))]
    [XmlElement(XmlSkillConditionParameterNames.WeaponType, typeof(XmlSkillStringList))]
    [XmlElement(XmlSkillConditionParameterNames.WeightPercent, typeof(byte))]
    [XmlChoiceIdentifier(nameof(ParameterTypes))]
    public object[]? Parameters { get; set; }

    [XmlIgnore]
    public XmlSkillConditionParameterType[]? ParameterTypes { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("type")]
    public string Type { get; set; } = string.Empty;
}