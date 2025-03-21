using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Teleporters;

[XmlRoot("list")]
public class XmlTeleporterList
{
    [XmlElement("npc")]
    public List<XmlTeleporter> Npcs { get; set; } = [];
}