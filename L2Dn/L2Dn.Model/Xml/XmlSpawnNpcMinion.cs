using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlSpawnNpcMinion 
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; } = 1;

    [XmlAttribute("max")]
    public int MaxCount { get; set; }

    [XmlAttribute("respawnTime")]
    public string RespawnTime { get; set; } = string.Empty;

    [XmlAttribute("randomRespawn")]
    public string RandomRespawn { get; set; } = string.Empty;

    [XmlAttribute("weightpoint")]
    public int WeightPoint { get; set; }
}