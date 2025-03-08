using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlElementalSpiritData
{
    [XmlElement("spirit")]
    public List<XmlElementalSpirit> Spirits { get; set; } = [];
}