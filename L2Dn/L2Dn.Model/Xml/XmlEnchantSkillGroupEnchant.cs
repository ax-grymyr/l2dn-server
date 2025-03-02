using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantSkillGroupEnchant
{
    [XmlArray("sps")]
    [XmlArrayItem("sp")]
    public List<XmlEnchantSkillGroupEnchantSp> Sps { get; set; } = [];

    [XmlArray("chances")]
    [XmlArrayItem("chance")]
    public List<XmlEnchantSkillGroupEnchantChance> Chances { get; set; } = [];

    [XmlArray("items")]
    [XmlArrayItem("item")]
    public List<XmlEnchantSkillGroupEnchantItem> Items { get; set; } = [];

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("enchantFailLevel")]
    public int EnchantFailLevel { get; set; }
}