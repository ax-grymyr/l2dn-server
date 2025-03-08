using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlEnchantItemGroupData
{
    [XmlElement("enchantRateGroup")]
    public List<XmlEnchantRateGroup> EnchantRateGroups { get; set; } = [];

    [XmlElement("enchantScrollGroup")]
    public List<XmlEnchantScrollGroup> EnchantScrollGroups { get; set; } = [];
}