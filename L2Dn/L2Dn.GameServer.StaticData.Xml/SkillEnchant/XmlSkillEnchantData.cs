using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.SkillEnchant;

[XmlRoot("list")]
public sealed class XmlSkillEnchantData
{
    [XmlArray("skills")]
    [XmlArrayItem("skill")]
    public List<XmlSkillEnchantSkill> Skills { get; set; } = [];

    [XmlArray("stars")]
    [XmlArrayItem("star")]
    public List<XmlSkillEnchantStar> Stars { get; set; } = [];

    [XmlArray("chances")]
    [XmlArrayItem("chance")]
    public List<XmlSkillEnchantChance> Chances { get; set; } = [];

    [XmlArray("itemsPoints")]
    [XmlArrayItem("star")]
    public List<XmlSkillEnchantPointStar> ItemPoints { get; set; } = [];
}