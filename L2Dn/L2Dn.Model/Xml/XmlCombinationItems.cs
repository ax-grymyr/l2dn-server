using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCombinationItems
{
    [XmlElement("item")]
    public List<XmlCombinationItem> Items { get; set; } = [];
}