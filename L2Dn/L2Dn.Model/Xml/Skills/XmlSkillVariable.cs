using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillVariable: XmlSkillValue
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}