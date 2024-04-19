using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlAgathionData: XmlBase
{
    [XmlElement("agathion")]
    public List<XmlAgathion> Agathions { get; set; } = [];
}