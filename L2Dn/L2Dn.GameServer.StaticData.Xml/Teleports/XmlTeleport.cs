using System.Xml.Serialization;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.GameServer.StaticData.Xml.Teleports;

public sealed class XmlTeleport: XmlLocation3D
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("price")]
    public int Price { get; set; }

    [XmlAttribute("special")]
    public bool Special { get; set; }
}