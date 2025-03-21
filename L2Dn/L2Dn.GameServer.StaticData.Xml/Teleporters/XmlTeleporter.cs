using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Teleporters;

public class XmlTeleporter
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlArray("npcs")]
    [XmlArrayItem("npc")]
    public List<XmlTeleporterNpcInner> Npcs { get; set; } = [];

    [XmlElement("teleport")]
    public List<XmlTeleport> Teleports { get; set; } = [];
}