using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlDoorList
{
    [XmlElement("door")]
    public List<XmlDoor> Doors { get; set; } = [];
}