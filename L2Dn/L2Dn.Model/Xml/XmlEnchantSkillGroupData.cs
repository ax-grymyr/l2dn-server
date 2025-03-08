using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlEnchantSkillGroupData
{
    [XmlElement("enchant")]
    public List<XmlEnchantSkillGroupEnchant> Enchants { get; set; } = [];
}