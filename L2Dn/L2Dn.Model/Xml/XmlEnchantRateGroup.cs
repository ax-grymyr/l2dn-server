using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantRateGroup
{
    [XmlElement("current")]
    public List<XmlEnchantRateGroupCurrent> Chances { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}