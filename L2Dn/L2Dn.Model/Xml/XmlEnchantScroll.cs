using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlEnchantScroll
{
    [XmlElement("step")]
    public List<XmlEnchantScrollStep> Steps { get; set; } = [];

    [XmlElement("item")]
    public List<XmlEnchantScrollItem> Items { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("minEnchant")]
    public int MinEnchant { get; set; }

    [XmlAttribute("maxEnchant")]
    public int MaxEnchant { get; set; } = 127;

    [XmlAttribute("randomEnchantMin")]
    public int RandomEnchantMin { get; set; } = 1;

    [XmlAttribute("randomEnchantMax")]
    public int RandomEnchantMax { get; set; } = 1;

    [XmlAttribute("maxEnchantFighter")]
    public int MaxEnchantFighter { get; set; }

    [XmlAttribute("maxEnchantMagic")]
    public int MaxEnchantMagic { get; set; }

    [XmlAttribute("safeEnchant")]
    public int SafeEnchant { get; set; }

    [XmlAttribute("bonusRate")]
    public double BonusRate { get; set; }

    [XmlAttribute("targetGrade")]
    public CrystalType TargetGrade { get; set; }

    [XmlAttribute("isBlessed")] // TODO Not present in xsd
    public bool IsBlessed { get; set; }

    [XmlAttribute("scrollGroupId")]
    public int ScrollGroupId { get; set; }
}