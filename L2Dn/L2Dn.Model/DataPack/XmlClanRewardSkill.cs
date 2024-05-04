using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlClanRewardSkill
{
    [XmlAttribute("id")]
    public int SkillId { get; set; }

    [XmlAttribute("level")]
    public int SkillLevel { get; set; }
}