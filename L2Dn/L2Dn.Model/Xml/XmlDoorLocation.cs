using System.Xml.Serialization;
using L2Dn.GameServer.StaticData.Xml.Common;

namespace L2Dn.Model.Xml;

public class XmlDoorLocation: XmlLocation
{
    [XmlAttribute("height")]
    public int Height { get; set; }

    [XmlIgnore]
    public bool HeightSpecified { get; set; }
}