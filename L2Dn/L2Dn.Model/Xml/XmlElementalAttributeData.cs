using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlElementalAttributeData
{
    [XmlElement("item")]
    public List<XmlElementalAttributeItem> Items { get; set; } = [];
}