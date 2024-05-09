using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEnsoulStoneData: XmlBase
{
    [XmlElement("stone")]
    public List<XmlEnsoulStone> Stones { get; set; } = [];
}