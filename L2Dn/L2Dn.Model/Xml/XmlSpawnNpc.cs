using System.Xml.Serialization;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.Model.Xml;

public class XmlSpawnNpc: XmlLocationWithHeading
{
    [XmlArray("parameters")]
    [XmlArrayItem("minions", typeof(XmlSpawnNpcParameterMinions))]
    [XmlArrayItem("param", typeof(XmlParameterString))]
    [XmlArrayItem("skill", typeof(XmlParameterSkill))]
    public List<XmlParameter> Parameters { get; set; } = [];

    [XmlArray("minions")]
    [XmlArrayItem("minion")]
    public List<XmlSpawnNpcMinion> Minions { get; set; } = [];

    [XmlArray("locations")]
    [XmlArrayItem("location")]
    public List<XmlSpawnNpcLocation> Locations { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("zone")]
    public string Zone { get; set; } = string.Empty;

    [XmlAttribute("count")]
    public int Count { get; set; } = 1;

    [XmlAttribute("respawnTime")]
    public string RespawnTime { get; set; } = string.Empty;

    [XmlAttribute("respawnRandom")]
    public string RespawnRandom { get; set; } = string.Empty;

    [XmlAttribute("respawnPattern")]
    public string RespawnPattern { get; set; } = string.Empty;

    [XmlAttribute("chaseRange")]
    public int ChaseRange { get; set; }

    [XmlAttribute("spawnAnimation")]
    public bool SpawnAnimation { get; set; }

    [XmlAttribute("dbName")]
    public string DbName { get; set; } = string.Empty;

    [XmlAttribute("dbSave")]
    public bool DbSave { get; set; }
}