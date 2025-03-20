using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillVariable: XmlSkillValue
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}