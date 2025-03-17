using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Common;

public class XmlLocation3D: XmlLocation2D
{
    [XmlAttribute]
    public int Z { get; set; }
}