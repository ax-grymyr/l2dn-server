using System.Xml.Serialization;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemSkill
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("type")]
    public ItemSkillType Type { get; set; }

    [XmlAttribute("type_chance")]
    public int Chance { get; set; }

    [XmlAttribute("type_value")]
    public int Value { get; set; }
}