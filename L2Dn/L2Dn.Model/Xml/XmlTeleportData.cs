using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlTeleportData
{
    [XmlElement("npc")]
    public List<XmlTeleporterNpc> Npcs { get; set; } = [];
}