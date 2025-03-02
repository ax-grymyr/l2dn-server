using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

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