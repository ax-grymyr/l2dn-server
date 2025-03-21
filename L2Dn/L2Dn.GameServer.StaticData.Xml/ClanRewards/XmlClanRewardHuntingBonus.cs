using System.Xml.Serialization;
using L2Dn.Model.Xml;

namespace L2Dn.GameServer.StaticData.Xml.ClanRewards;

public class XmlClanRewardHuntingBonus
{
    [XmlElement("skill")]
    public XmlClanRewardSkill? Skill { get; set; }

    [XmlAttribute("points")]
    public int Points { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}