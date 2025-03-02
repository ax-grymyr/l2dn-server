using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlMapRegionList: XmlBase
{
    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }

    [XmlElement("region")]
    public List<XmlMapRegion> Regions { get; set; } = [];
}