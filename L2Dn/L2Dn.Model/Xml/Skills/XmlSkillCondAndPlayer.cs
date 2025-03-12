using System.Xml.Serialization;

namespace L2Dn.Model.Xml.Skills;

public class XmlSkillCondAndPlayer
{
    [XmlAttribute("canEscape")]
    public bool CanEscape { get; set; }
}