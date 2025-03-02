using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlParameterString: XmlParameter
{
    [XmlAttribute("value")]
    public string Value { get; set; } = string.Empty;
}