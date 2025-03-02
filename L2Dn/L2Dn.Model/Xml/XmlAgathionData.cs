using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlAgathionData: XmlBase
{
    [XmlElement("agathion")]
    public List<XmlAgathion> Agathions { get; set; } = [];
}