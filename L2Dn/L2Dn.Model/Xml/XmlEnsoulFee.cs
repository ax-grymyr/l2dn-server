using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlEnsoulFee
{
    [XmlElement("first")]
    public XmlEnsoulFeeItem? First { get; set; }

    [XmlElement("secondary")]
    public XmlEnsoulFeeItem? Secondary { get; set; }

    [XmlElement("third")]
    public XmlEnsoulFeeItem? Third { get; set; }

    [XmlElement("reNormal")]
    public XmlEnsoulFeeItem? ReNormal { get; set; }

    [XmlElement("reSecondary")]
    public XmlEnsoulFeeItem? ReSecondary { get; set; }

    [XmlElement("reThird")]
    public XmlEnsoulFeeItem? ReThird { get; set; }

    [XmlElement("remove")]
    public List<XmlEnsoulFeeItem> Remove { get; set; } = [];

    [XmlAttribute("crystalType")]
    public CrystalType CrystalType { get; set; }
}