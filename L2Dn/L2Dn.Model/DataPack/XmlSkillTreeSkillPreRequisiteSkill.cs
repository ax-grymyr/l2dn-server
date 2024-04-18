using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlSkillTreeSkillPreRequisiteSkill 
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("lvl")]
    public int Level { get; set; }
}