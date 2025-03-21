using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Categories;

public class XmlCategory
{
    [XmlAttribute("name")]
    public CategoryType Type { get; set; }

    [XmlElement("id")]
    public List<int> Ids { get; set; } = [];
}