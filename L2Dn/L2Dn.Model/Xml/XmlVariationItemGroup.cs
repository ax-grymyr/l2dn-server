using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlVariationItemGroup
{
    [XmlElement("item")]
    public List<XmlId> Items { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlIgnore]
    public bool IdSpecified { get; set; }
}