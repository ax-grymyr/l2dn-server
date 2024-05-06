using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlElementalSpiritData: XmlBase
{
    [XmlElement("spirit")]
    public List<XmlElementalSpirit> Spirits { get; set; } = [];
}