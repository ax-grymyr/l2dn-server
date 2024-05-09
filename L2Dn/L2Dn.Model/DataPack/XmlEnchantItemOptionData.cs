using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEnchantItemOptionData: XmlBase
{
    [XmlElement("item")]
    public List<XmlEnchantItemOptionItem> Items { get; set; } = [];
}