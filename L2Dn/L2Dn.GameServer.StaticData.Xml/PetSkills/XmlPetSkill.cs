using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetSkills;

public sealed class XmlPetSkill
{
    [XmlAttribute("npcId")]
    public int NpcId { get; set; }

    [XmlAttribute("skillId")]
    public int SkillId { get; set; }

    [XmlAttribute("skillLevel")]
    public int SkillLevel { get; set; }
}