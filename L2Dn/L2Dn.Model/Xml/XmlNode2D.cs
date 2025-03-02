using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlNode2D
{
    [XmlAttribute("x")]
    public int X { get; set; }

    [XmlAttribute("y")]
    public int Y { get; set; }
}