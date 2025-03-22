using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetSkills;

[XmlRoot("list")]
public sealed class XmlPetSkillList
{
    [XmlElement("skill")]
    public List<XmlPetSkill> Skills { get; set; } = [];
}