using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlPlayerXpPercentLostData: XmlBase
{
    [XmlElement("xpLost")]
    public List<XmlPlayerXpPercentLost> Levels { get; set; } = [];
}