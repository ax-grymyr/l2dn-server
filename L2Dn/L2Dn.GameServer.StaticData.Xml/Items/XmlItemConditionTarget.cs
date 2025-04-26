using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemConditionTarget
{
    [XmlAttribute("levelRange")]
    public string? LevelRange { get; set; }

    [XmlAttribute("categoryType")]
    public string? CategoryType { get; set; }

    [XmlAttribute("aggro")]
    public bool Aggro { get; set; }

    [XmlIgnore]
    public bool AggroSpecified { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlIgnore]
    public bool LevelSpecified { get; set; }

    [XmlAttribute("siegeZone")]
    public int SiegeZone { get; set; }

    [XmlIgnore]
    public bool SiegeZoneSpecified { get; set; }

    [XmlAttribute("myPartyExceptMe")]
    public bool MyPartyExceptMe { get; set; }

    [XmlIgnore]
    public bool MyPartyExceptMeSpecified { get; set; }

    [XmlAttribute("playable")]
    public bool Playable { get; set; }

    [XmlIgnore]
    public bool PlayableSpecified { get; set; }

    [XmlAttribute("player")]
    public bool Player { get; set; }

    [XmlIgnore]
    public bool PlayerSpecified { get; set; }

    [XmlAttribute("checkCrtEffect")]
    public bool CheckCrtEffect { get; set; }

    [XmlIgnore]
    public bool CheckCrtEffectSpecified { get; set; }

    [XmlAttribute("invSize")]
    public int InventorySize { get; set; }

    [XmlIgnore]
    public bool InventorySizeSpecified { get; set; }

    [XmlAttribute("weight")]
    public int Weight { get; set; }

    [XmlIgnore]
    public bool WeightSpecified { get; set; }

    [XmlAttribute("minDistance")]
    public int MinDistance { get; set; }

    [XmlIgnore]
    public bool MinDistanceSpecified { get; set; }

    [XmlAttribute("race")]
    public Race Race { get; set; }

    [XmlIgnore]
    public bool RaceSpecified { get; set; }

    [XmlAttribute("abnormalType")]
    public AbnormalType AbnormalType { get; set; }

    [XmlIgnore]
    public bool AbnormalTypeSpecified { get; set; }

    [XmlAttribute("classIdRestriction")]
    public string? ClassIdRestriction { get; set; }

    [XmlAttribute("active_effect_id")]
    public int ActiveEffectId { get; set; }

    [XmlIgnore]
    public bool ActiveEffectIdSpecified { get; set; }

    [XmlAttribute("active_effect_id_lvl")]
    public string? ActiveEffectIdLevel { get; set; }

    [XmlAttribute("active_skill_id")]
    public int ActiveSkillId { get; set; }

    [XmlIgnore]
    public bool ActiveSkillIdSpecified { get; set; }

    [XmlAttribute("active_skill_id_lvl")]
    public string? ActiveSkillIdLevel { get; set; }

    [XmlAttribute("using")]
    public string? Using { get; set; }

    [XmlAttribute("npcId")]
    public string? NpcId { get; set; }

    [XmlAttribute("npcType")]
    public string? NpcType { get; set; }
}