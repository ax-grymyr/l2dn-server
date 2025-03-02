using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlCastle
{
    [XmlArray("spawns")]
    [XmlArrayItem("npc")]
    public List<XmlCastleSpawn> Spawns { get; set; } = [];

    [XmlArray("siegeGuards")]
    [XmlArrayItem("guard")]
    public List<XmlCastleSiegeGuard> SiegeGuards { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlIgnore]
    public bool IdSpecified { get; set; }
}