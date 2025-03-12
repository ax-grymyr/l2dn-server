namespace L2Dn.Model.Xml.Skills;

public interface IXmlSkillLevelRestriction
{
    byte Level { get; }
    bool LevelSpecified { get; }
    byte FromLevel { get; }
    bool FromLevelSpecified { get; }
    byte ToLevel { get; }
    bool ToLevelSpecified { get; }
    ushort FromSubLevel { get; }
    bool FromSubLevelSpecified { get; }
    ushort ToSubLevel { get; }
    bool ToSubLevelSpecified { get; }
}