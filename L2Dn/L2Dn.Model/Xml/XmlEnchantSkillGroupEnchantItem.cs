using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlEnchantSkillGroupEnchantItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; }

    [XmlAttribute("type")]
    public SkillEnchantType Type { get; set; }
}