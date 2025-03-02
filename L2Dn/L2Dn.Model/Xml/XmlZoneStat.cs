using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlZoneStat
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("val")]
    public string Value { get; set; } = string.Empty;
}