using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.TimedHuntingZones;

[XmlRoot("list")]
public class XmlTimedHuntingZoneList
{
    [XmlElement("zone")]
    public List<XmlTimedHuntingZone> Zones { get; set; } = [];

    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }

    [XmlIgnore]
    public bool EnabledSpecified { get; set; }
}