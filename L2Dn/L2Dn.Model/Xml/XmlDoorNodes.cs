using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlDoorNodes
{
    [XmlElement("node")]
    public List<XmlNode2D> Nodes { get; set; } = [];

    [XmlAttribute("nodeZ")]
    public int NodeZ { get; set; }
}