using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlElementalAttributeData: XmlBase
{
    [XmlElement("item")]
    public List<XmlElementalAttributeItem> Items { get; set; } = [];
}