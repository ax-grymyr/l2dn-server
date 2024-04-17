using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlZone
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("id")]
    public int Id { get; set; }
    
    [XmlIgnore]
    public bool IdSpecified { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; } = string.Empty;

    [XmlAttribute("shape")]
    public string Shape { get; set; } = string.Empty;

    [XmlAttribute("minZ")]
    public int MinZ { get; set; }

    [XmlAttribute("maxZ")]
    public int MaxZ { get; set; }

    [XmlAttribute("rad")]
    public int Radius { get; set; }

    [XmlElement("stat")]
    public List<XmlZoneStat> Stats { get; set; } = [];

    [XmlElement("node")]
    public List<XmlZoneNode> Nodes { get; set; } = [];

    [XmlElement("race")]
    public List<XmlZoneRace> Races { get; set; } = [];

    [XmlElement("spawn")]
    public List<XmlZoneSpawn> Spawns { get; set; } = [];
}