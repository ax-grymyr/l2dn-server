using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Common;

public class XmlLocation
{
    [XmlAttribute("x")]
    public int X { get; set; }

    [XmlIgnore]
    public bool XSpecified { get; set; }

    [XmlAttribute("y")]
    public int Y { get; set; }

    [XmlIgnore]
    public bool YSpecified { get; set; }

    [XmlAttribute("z")]
    public int Z { get; set; }

    [XmlIgnore]
    public bool ZSpecified { get; set; }
}