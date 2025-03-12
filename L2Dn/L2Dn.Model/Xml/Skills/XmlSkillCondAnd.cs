namespace L2Dn.Model.Xml.Skills;

public class XmlSkillCondAnd
{
    public XmlSkillCondAndPlayer player { get; set; } = new();

    public XmlSkillCondAndNot not { get; set; } = new();
}