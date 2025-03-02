using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlEnchantSkillGroupEnchantChance
{
    [XmlAttribute("value")]
    public int Value { get; set; }

    [XmlAttribute("type")]
    public SkillEnchantType Type { get; set; }
}