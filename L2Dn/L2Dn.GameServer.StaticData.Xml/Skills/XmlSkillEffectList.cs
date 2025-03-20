using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillEffectList
{
    [XmlElement("effect")]
    public List<XmlSkillEffect> Effects { get; set; } = [];
}