using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlElementalSpiritLevel
{
    [XmlAttribute("id")]
    public int Level { get; set; }

    [XmlAttribute("atk")]
    public int Attack { get; set; }

    [XmlAttribute("def")]
    public int Defense { get; set; }

    [XmlAttribute("critRate")]
    public int CriticalRate { get; set; }

    [XmlAttribute("critDam")]
    public int CriticalDamage { get; set; }

    [XmlAttribute("maxExp")]
    public long MaxExperience { get; set; }
}