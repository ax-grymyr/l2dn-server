namespace L2Dn.GameServer.StaticData.Xml.Skills;

public interface IXmlSkillValue
{
    IReadOnlyList<IXmlSkillLevelValue> Values { get; }
    string Value { get; }
}