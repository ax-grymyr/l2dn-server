using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantItemOptionItem
{
    [XmlElement("options")]
    public List<XmlEnchantItemOption> Options { get; set; } = [];

    [XmlAttribute("id")]
    public int ItemId { get; set; }
}