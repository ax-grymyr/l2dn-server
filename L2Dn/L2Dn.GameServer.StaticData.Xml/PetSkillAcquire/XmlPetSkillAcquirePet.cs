using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetSkillAcquire;

public sealed class XmlPetSkillAcquirePet
{
    [XmlElement("skill")]
    public List<XmlPetSkillAcquirePetSkill> Skills { get; set; } = [];

    [XmlAttribute("type")]
    public int Type { get; set; }

    [XmlIgnore]
    public bool TypeSpecified { get; set; }
}