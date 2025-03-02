using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlCategory
{
    [XmlAttribute("name")]
    public CategoryType Type { get; set; }

    [XmlElement("id")]
    public List<int> Ids { get; set; } = [];
}