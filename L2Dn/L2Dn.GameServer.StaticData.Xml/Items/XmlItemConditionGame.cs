using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemConditionGame
{
    [XmlAttribute("night")]
    public bool Night { get; set; }

    [XmlIgnore]
    public bool NightSpecified { get; set; }

    [XmlAttribute("skill")]
    public bool Skill { get; set; }

    [XmlIgnore]
    public bool SkillSpecified { get; set; }

    [XmlAttribute("chance")]
    public int Chance { get; set; }

    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}