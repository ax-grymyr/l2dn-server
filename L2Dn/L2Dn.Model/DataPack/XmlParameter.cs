using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public abstract class XmlParameter
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}