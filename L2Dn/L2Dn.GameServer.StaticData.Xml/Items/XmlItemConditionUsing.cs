using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemConditionUsing
{
    [XmlAttribute("kind")]
    public string? Kind { get; set; }

    [XmlAttribute("slot")]
    public string? Slot { get; set; }

    [XmlAttribute("weaponChange")]
    public bool WeaponChange { get; set; }

    [XmlIgnore]
    public bool WeaponChangeSpecified { get; set; }

    [XmlAttribute("skill")]
    public int Skill { get; set; }

    [XmlIgnore]
    public bool SkillSpecified { get; set; }

    [XmlAttribute("slotItem")]
    public string? SlotItem { get; set; }
}