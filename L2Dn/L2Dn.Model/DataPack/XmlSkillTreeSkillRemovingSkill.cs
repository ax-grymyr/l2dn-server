using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlSkillTreeSkillRemovingSkill 
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("onlyReplaceByLearn")]
    public bool OnlyReplaceByLearn { get; set; }
}