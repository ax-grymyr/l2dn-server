using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantChallengePointGroup
{
    [XmlElement("option")]
    public List<XmlEnchantChallengePointGroupOption> Options { get; set; } = [];

    [XmlElement("item")]
    public List<XmlEnchantChallengePointGroupItem> Items { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }
}