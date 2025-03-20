using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

[XmlRoot("list")]
public class XmlSkillList: IXmlRoot
{
    [XmlElement("skill")]
    public List<XmlSkill> Skills { get; set; } = [];

    [XmlIgnore]
    public string FilePath { get; set; } = string.Empty;
}