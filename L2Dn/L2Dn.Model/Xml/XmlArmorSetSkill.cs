using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlArmorSetSkill
{
    [XmlAttribute("id")]
    public int SkillId { get; set; }

    [XmlAttribute("level")]
    public int SkillLevel { get; set; }

    [XmlAttribute("minimumPieces")]
    public int MinimumPieces { get; set; }

    [XmlIgnore]
    public bool MinimumPiecesSpecified { get; set; }

    [XmlAttribute("minimumEnchant")]
    public int MinimumEnchant { get; set; }

    [XmlIgnore]
    public bool MinimumEnchantSpecified { get; set; }

    [XmlAttribute("optional")]
    public bool Optional { get; set; }

    [XmlIgnore]
    public bool OptionalSpecified { get; set; }

    [XmlAttribute("slotMask")]
    public int SlotMask { get; set; }

    [XmlAttribute("bookSlot")]
    public int BookSlot { get; set; }
}