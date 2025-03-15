using System.Xml.Serialization;
using L2Dn.GameServer.Enums;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlSkillTreeSkill
{
    [XmlElement("subClassConditions")]
    public List<XmlSkillTreeSkillSubClassCondition> SubClassConditions { get; set; } = [];

    [XmlElement("removeSkill")]
    public List<XmlSkillTreeSkillRemovingSkill> RemovingSkills { get; set; } = [];

    [XmlElement("socialClass")]
    public SocialClass SocialClass { get; set; }

    [XmlIgnore]
    public bool SocialClassSpecified { get; set; }

    [XmlElement("residenceId")]
    public List<int> ResidenceIds { get; set; } = [];

    [XmlElement("race")]
    public List<Race> Races { get; set; } = [];

    [XmlElement("preRequisiteSkill")]
    public List<XmlSkillTreeSkillPreRequisiteSkill> PreRequisiteSkills { get; set; } = [];

    [XmlElement("item")]
    public List<XmlSkillTreeSkillItem> Items { get; set; } = [];

    [XmlAttribute("autoGet")]
    public bool AutoGet { get; set; }

    [XmlAttribute("getLevel")]
    public int GetLevel { get; set; }

    [XmlAttribute("getDualClassLevel")]
    public int GetDualClassLevel { get; set; }

    [XmlAttribute("learnedByFS")]
    public bool LearnedByFs { get; set; }

    [XmlAttribute("learnedByNpc")]
    public bool LearnedByNpc { get; set; }

    [XmlAttribute("levelUpSp")]
    public long LevelUpSp { get; set; }

    [XmlAttribute("residenceSkill")]
    public bool ResidenceSkill { get; set; }

    [XmlAttribute("skillId")]
    public int SkillId { get; set; }

    [XmlAttribute("skillLevel")]
    public int SkillLevel { get; set; }

    [XmlAttribute("skillName")]
    public string SkillName { get; set; } = string.Empty;

    [XmlAttribute("treeId")]
    public int TreeId { get; set; }

    [XmlAttribute("row")]
    public int Row { get; set; }

    [XmlAttribute("column")]
    public int Column { get; set; }

    [XmlAttribute("pointsRequired")]
    public int PointsRequired { get; set; }
}