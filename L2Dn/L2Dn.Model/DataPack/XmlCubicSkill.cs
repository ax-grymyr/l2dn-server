using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlCubicSkill 
{
    [XmlElement("conditions")]
    public XmlCubicSkillConditions? Conditions { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("triggerRate")]
    public int TriggerRate { get; set; } = 100;

    [XmlIgnore]
    public bool TriggerRateSpecified { get; set; }

    [XmlAttribute("successRate")]
    public int SuccessRate { get; set; } = 100;

    [XmlIgnore]
    public bool SuccessRateSpecified { get; set; }
    
    [XmlAttribute("canUseOnStaticObjects")]
    public bool CanUseOnStaticObjects { get; set; }

    [XmlAttribute("target")]
    public CubicTargetType Target { get; set; }

    [XmlIgnore]
    public bool TargetSpecified { get; set; }

    [XmlAttribute("targetDebuff")]
    public bool TargetDebuff { get; set; }

    [XmlAttribute("range")]
    public int Range { get; set; }
}