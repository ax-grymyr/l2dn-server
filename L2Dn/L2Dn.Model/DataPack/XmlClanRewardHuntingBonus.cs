using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlClanRewardHuntingBonus
{
    [XmlElement("skill")]
    public XmlClanRewardSkill? Skill { get; set; }

    [XmlAttribute("points")]
    public int Points { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}