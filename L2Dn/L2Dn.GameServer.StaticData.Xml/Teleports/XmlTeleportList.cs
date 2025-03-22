using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Teleports;

[XmlRoot("list")]
public sealed class XmlTeleportList
{
    [XmlElement("teleport")]
    public List<XmlTeleport> Teleports { get; set; } = [];
}