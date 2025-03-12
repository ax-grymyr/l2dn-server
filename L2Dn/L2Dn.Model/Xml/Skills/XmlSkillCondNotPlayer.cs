using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillCondNotPlayer
{
    [XmlAttribute("insideZoneId")]
    public string InsideZoneId { get; set; } = string.Empty;
}