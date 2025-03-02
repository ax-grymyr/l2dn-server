using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("table")]
public class XmlExperienceData: XmlBase
{
    [XmlAttribute("maxLevel")]
    public int MaxLevel { get; set; }

    [XmlAttribute("maxPetLevel")]
    public int MaxPetLevel { get; set; }

    [XmlElement("experience")]
    public List<XmlExperience> Levels { get; set; } = [];
}