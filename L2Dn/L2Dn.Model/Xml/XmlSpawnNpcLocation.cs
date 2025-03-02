using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlSpawnNpcLocation: XmlLocationWithHeading
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }
}