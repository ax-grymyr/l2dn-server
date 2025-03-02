using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlSpawn
{
    [XmlArray("territories")]
    [XmlArrayItem("territory", typeof(XmlSpawnTerritory))]
    [XmlArrayItem("banned_territory", typeof(XmlSpawnBannedTerritory))]
    public List<XmlSpawnTerritoryBase> Territories { get; set; } = [];

    [XmlArray("parameters")]
    [XmlArrayItem("minions", typeof(XmlSpawnNpcParameterMinions))]
    [XmlArrayItem("param", typeof(XmlParameterString))]
    [XmlArrayItem("skill", typeof(XmlParameterSkill))]
    public List<XmlParameter> Parameters { get; set; } = [];

    [XmlElement("npc")]
    public List<XmlSpawnNpc> Npcs { get; set; } = [];

    [XmlElement("group")]
    public List<XmlSpawnGroup> Groups { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("ai")]
    public string Ai { get; set; } = string.Empty;

    [XmlAttribute("spawnByDefault")]
    public bool SpawnByDefault { get; set; } = true;
}