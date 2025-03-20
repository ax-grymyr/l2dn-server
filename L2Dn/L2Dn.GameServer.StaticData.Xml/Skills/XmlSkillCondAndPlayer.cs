using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillCondAndPlayer
{
    [XmlAttribute("canEscape")]
    public bool CanEscape { get; set; }
}