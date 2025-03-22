using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Sayune;

public sealed class XmlSayuneItem
{
    [XmlElement("loc")]
    public List<XmlSayuneLocation> Locations { get; set; } = [];

    [XmlElement("selector")]
    public XmlSayuneSelector? Selector { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }
}