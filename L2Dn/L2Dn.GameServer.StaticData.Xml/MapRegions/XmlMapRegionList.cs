using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.MapRegions;

[XmlRoot("list")]
public sealed class XmlMapRegionList
{
    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }

    [XmlElement("region")]
    public List<XmlMapRegion> Regions { get; set; } = [];
}