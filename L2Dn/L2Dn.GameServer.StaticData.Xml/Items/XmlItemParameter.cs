using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemParameter
{
    [XmlAttribute("name")]
    public XmlItemParameterType Type { get; set; }

    [XmlAttribute("val")]
    public string Value { get; set; } = string.Empty;
}