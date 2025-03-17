using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Common;

public class XmlLocation2D
{
    [XmlAttribute]
    public int X { get; set; }

    [XmlAttribute]
    public int Y { get; set; }
}