using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEnsoulFeeData: XmlBase
{
    [XmlElement("fee")]
    public List<XmlEnsoulFee> Fees { get; set; } = [];
}