using System.Xml.Serialization;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.GameServer.StaticData.Xml.Zones;

public class XmlZoneSpawn: XmlLocation3D
{
    [XmlAttribute("type")]
    public XmlZoneSpawnType Type { get; set; }

    [XmlIgnore]
    public bool TypeSpecified { get; set; }
}