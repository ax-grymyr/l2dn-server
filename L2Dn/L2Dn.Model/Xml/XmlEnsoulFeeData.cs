using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlEnsoulFeeData
{
    [XmlElement("fee")]
    public List<XmlEnsoulFee> Fees { get; set; } = [];
}