using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEnchantItemData: XmlBase
{
    [XmlElement("enchant")]
    public List<XmlEnchantScroll> EnchantScrolls { get; set; } = [];
}