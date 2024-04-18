using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlSkillTreeSkillSubClassCondition 
{
    [XmlAttribute("slot")]
    public int Slot { get; set; }

    [XmlAttribute("lvl")]
    public int Level { get; set; }
}