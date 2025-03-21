using System.Xml.Serialization;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.Model.Xml;

public class XmlSpawnNpcLocation: XmlLocationWithHeading
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }
}