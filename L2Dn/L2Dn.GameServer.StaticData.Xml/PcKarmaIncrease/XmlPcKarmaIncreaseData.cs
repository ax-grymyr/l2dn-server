using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PcKarmaIncrease;

[XmlRoot("pcKarmaIncrease")]
public class XmlPcKarmaIncreaseData
{
    [XmlElement("increase")]
    public List<XmlPcKarmaIncrease> Levels { get; set; } = [];
}