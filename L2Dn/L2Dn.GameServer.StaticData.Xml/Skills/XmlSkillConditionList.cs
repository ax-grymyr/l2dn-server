using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillConditionList
{
    [XmlElement("condition")]
    public List<XmlSkillCondition> Conditions { get; set; } = [];
}