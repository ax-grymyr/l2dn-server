using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlAgathionData
{
    [XmlElement("agathion")]
    public List<XmlAgathion> Agathions { get; set; } = [];
}