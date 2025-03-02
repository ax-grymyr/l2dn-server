using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlEnsoulStoneData: XmlBase
{
    [XmlElement("stone")]
    public List<XmlEnsoulStone> Stones { get; set; } = [];
}