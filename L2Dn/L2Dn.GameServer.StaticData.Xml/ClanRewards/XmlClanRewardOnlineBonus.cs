using System.Xml.Serialization;
using L2Dn.Model.Xml;

namespace L2Dn.GameServer.StaticData.Xml.ClanRewards;

public class XmlClanRewardOnlineBonus
{
    [XmlElement("skill")]
    public XmlClanRewardSkill? Skill { get; set; }

    [XmlAttribute("size")]
    public int Count { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }
}