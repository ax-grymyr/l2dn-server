using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlEnchantSkillGroupEnchantSp
{
    [XmlAttribute("amount")]
    public long Amount { get; set; }

    [XmlAttribute("type")]
    public SkillEnchantType Type { get; set; }
}