using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Teleports;

public sealed class XmlTeleport
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("x")]
    public int X { get; set; }

    [XmlAttribute("y")]
    public int Y { get; set; }

    [XmlAttribute("z")]
    public int Z { get; set; }

    [XmlAttribute("price")]
    public int Price { get; set; }

    [XmlAttribute("special")]
    public bool Special { get; set; }
}