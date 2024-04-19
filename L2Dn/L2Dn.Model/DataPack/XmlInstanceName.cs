using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlInstanceName
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}