using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetSkillAcquire;

public sealed class XmlPetSkillAcquirePetSkill
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("lvl")]
    public int Level { get; set; }

    [XmlIgnore]
    public bool LevelSpecified { get; set; }

    [XmlAttribute("reqLvl")]
    public int RequiredLevel { get; set; }

    [XmlIgnore]
    public bool RequiredLevelSpecified { get; set; }

    [XmlAttribute("evolve")]
    public int Evolve { get; set; }

    [XmlIgnore]
    public bool EvolveSpecified { get; set; }

    [XmlAttribute("item")]
    public int ItemId { get; set; }

    [XmlIgnore]
    public bool ItemIdSpecified { get; set; }

    [XmlAttribute("itemAmount")]
    public long ItemAmount { get; set; }

    [XmlIgnore]
    public bool ItemAmountSpecified { get; set; }
}