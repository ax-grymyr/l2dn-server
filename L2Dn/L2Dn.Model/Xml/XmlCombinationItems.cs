using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCombinationItems: XmlBase
{
    [XmlElement("item")]
    public List<XmlCombinationItem> Items { get; set; } = [];
}