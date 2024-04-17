using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlAccessLevels: XmlBase
{
    [XmlElement("access")]
    public List<XmlAccessLevel> AccessLevels { get; set; } = [];
}