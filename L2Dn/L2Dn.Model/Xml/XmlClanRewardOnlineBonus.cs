using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlClanRewardOnlineBonus
{
    [XmlElement("skill")]
    public XmlClanRewardSkill? Skill { get; set; }

    [XmlAttribute("size")]
    public int Count { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}