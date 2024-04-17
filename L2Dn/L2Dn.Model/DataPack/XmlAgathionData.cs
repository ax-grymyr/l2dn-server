using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlAgathionData: XmlBase
{
    [XmlElement("agathion")]
    public List<XmlAgathion> Agathions { get; set; } = [];
}

public class XmlAgathion
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("enchant")]
    public int Enchant { get; set; }

    [XmlAttribute("mainSkill")]
    public string MainSkill { get; set; } = string.Empty;

    [XmlAttribute("subSkill")]
    public string SubSkill { get; set; } = string.Empty;
}