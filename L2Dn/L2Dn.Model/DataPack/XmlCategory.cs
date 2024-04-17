using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlCategory
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("id")]
    public List<int> Ids { get; set; } = [];
}