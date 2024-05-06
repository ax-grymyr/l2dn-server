using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantChallengePointGroupItem
{
    [XmlElement("enchant")]
    public List<XmlEnchantChallengePointGroupItemEnchant> Enchants { get; set; } = [];

    [XmlAttribute("id")]
    public string IdList { get; set; } = string.Empty;
}