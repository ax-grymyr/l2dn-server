using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlInstanceName
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}