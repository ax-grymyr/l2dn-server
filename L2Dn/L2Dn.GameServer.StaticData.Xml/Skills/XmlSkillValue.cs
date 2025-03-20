using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillValue: IXmlSkillValue
{
    [XmlElement("value")]
    public List<XmlSkillLevelValue> Values { get; set; } = [];

    [XmlText]
    public string Value { get; set; } = string.Empty;

    IReadOnlyList<IXmlSkillLevelValue> IXmlSkillValue.Values => Values;
}

public class XmlSkillValue<T>: IXmlSkillValue
    where T: unmanaged
{
    [XmlElement("value")]
    public List<XmlSkillLevelValue<T>> Values { get; set; } = [];

    [XmlText]
    public string Value { get; set; } = string.Empty;

    IReadOnlyList<IXmlSkillLevelValue> IXmlSkillValue.Values => Values;
}