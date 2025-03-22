using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Sayune;

public class XmlSayuneLocation
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("x")]
    public int X { get; set; }

    [XmlAttribute("y")]
    public int Y { get; set; }

    [XmlAttribute("z")]
    public int Z { get; set; }
}