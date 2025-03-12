namespace L2Dn.Model.Xml.Skills;

public interface IXmlSkillValue
{
    IReadOnlyList<IXmlSkillLevelValue> Values { get; }
    string Value { get; }
}