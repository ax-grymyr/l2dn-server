using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.ClanRewards;

public class XmlClanRewardSkill
{
    [XmlAttribute("id")]
    public int SkillId { get; set; }

    [XmlAttribute("level")]
    public int SkillLevel { get; set; }
}