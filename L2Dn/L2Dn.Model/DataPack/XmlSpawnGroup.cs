using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlSpawnGroup 
{
    [XmlArray("territories")]
    [XmlArrayItem("territory", typeof(XmlSpawnTerritory))]
    [XmlArrayItem("banned_territory", typeof(XmlSpawnBannedTerritory))]
    public List<XmlSpawnTerritoryBase> Territories { get; set; } = [];

    [XmlElement("npc")]
    public List<XmlSpawnNpc> Npcs { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("spawnByDefault")]
    public bool SpawnByDefault { get; set; } = true;
}