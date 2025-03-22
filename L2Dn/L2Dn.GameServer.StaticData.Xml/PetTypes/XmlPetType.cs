using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.PetTypes;

public sealed class XmlPetType
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("skillId")]
    public int SkillId { get; set; }

    [XmlAttribute("skillLvl")]
    public int SkillLevel { get; set; }
}