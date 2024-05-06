using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlEnchantChallengePointGroupItemEnchant
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("points")]
    public int Points { get; set; }
}