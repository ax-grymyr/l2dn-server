using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlSpawnNpcParameterMinion
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlAttribute("max")]
    public int MaxCount { get; set; }

    [XmlAttribute("respawnTime")]
    public int RespawnTime { get; set; }

    [XmlAttribute("weightPoint")]
    public int WeightPoint { get; set; }
}