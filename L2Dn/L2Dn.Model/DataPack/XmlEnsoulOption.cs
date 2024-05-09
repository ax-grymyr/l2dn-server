using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnsoulOption
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("desc")]
    public string Description { get; set; } = string.Empty;

    [XmlAttribute("skillId")]
    public int SkillId { get; set; }

    [XmlAttribute("skillLevel")]
    public int SkillLevel { get; set; }
}