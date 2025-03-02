using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("pcKarmaIncrease")]
public class XmlPcKarmaIncreaseData: XmlBase
{
    [XmlElement("increase")]
    public List<XmlPcKarmaIncrease> Levels { get; set; } = [];
}