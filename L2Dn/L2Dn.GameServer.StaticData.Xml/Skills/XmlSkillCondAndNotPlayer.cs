using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillCondAndNotPlayer
{
    [XmlAttribute("insideZoneId")]
    public string InsideZoneId { get; set; } = string.Empty;
}