using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCubicData: XmlBase
{
    [XmlElement("cubic")]
    public List<XmlCubic> Cubics { get; set; } = [];
}