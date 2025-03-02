using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantItemOption
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("option1")]
    public int Option1 { get; set; } = -1;

    [XmlAttribute("option2")]
    public int Option2 { get; set; } = -1;

    [XmlAttribute("option3")]
    public int Option3 { get; set; } = -1;
}