namespace L2Dn.GameServer.StaticData.Xml.Skills;

public class XmlSkillCondAnd
{
    public XmlSkillCondAndPlayer player { get; set; } = new();

    public XmlSkillCondAndNot not { get; set; } = new();
}