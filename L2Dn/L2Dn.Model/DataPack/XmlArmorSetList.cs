using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlArmorSetList: XmlBase
{
    [XmlElement("access")]
    public List<XmlArmorSet> Sets { get; set; } = [];
}

public class XmlArmorSet
{
    [XmlArray("requiredItems")]
    [XmlArrayItem("item")]
    public List<XmlArmorSetItem> RequiredItems { get; set; } = [];

    [XmlArray("optionalItems")]
    [XmlArrayItem("item")]
    public List<XmlArmorSetItem> OptionalItems { get; set; } = [];

    [XmlArray("skills")]
    [XmlArrayItem("skill")]
    public List<XmlArmorSetSkill> Skills { get; set; } = [];

    [XmlArray("stats")]
    [XmlArrayItem("stat")]
    public List<XmlArmorSetStat> Stats { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("visual")]
    public bool Visual { get; set; }

    [XmlIgnore]
    public bool VisualSpecified { get; set; }

    [XmlAttribute("minimumPieces")]
    public int MinimumPieces { get; set; }
}

public class XmlArmorSetItem 
{
    [XmlAttribute("id")]
    public int Id { get; set; }
}

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

public class XmlArmorSetStat 
{
    [XmlAttribute("type")]
    public BaseStat Stat { get; set; }

    [XmlAttribute("val")]
    public double Value { get; set; }
}