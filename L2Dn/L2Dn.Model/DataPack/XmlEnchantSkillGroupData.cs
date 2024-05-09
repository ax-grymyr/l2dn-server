using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEnchantSkillGroupData: XmlBase
{
    [XmlElement("enchant")]
    public List<XmlEnchantSkillGroupEnchant> Enchants { get; set; } = [];
}