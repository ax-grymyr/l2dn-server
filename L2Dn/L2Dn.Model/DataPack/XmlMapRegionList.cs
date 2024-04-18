using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlMapRegionList: XmlBase
{
    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }
    
    [XmlElement("region")]
    public List<XmlMapRegion> Regions { get; set; } = [];
}