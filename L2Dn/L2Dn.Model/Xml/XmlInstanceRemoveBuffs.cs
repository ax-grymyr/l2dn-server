using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlInstanceRemoveBuffs
{
    [XmlElement("skill")]
    public List<XmlInstanceRemoveBuffsSkill> Skills { get; set; } = [];

    [XmlAttribute("type")]
    public InstanceRemoveBuffType Type { get; set; }
}