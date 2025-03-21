using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.ElementalSpirits;

[XmlRoot("list")]
public class XmlElementalSpiritList
{
    [XmlElement("spirit")]
    public List<XmlElementalSpirit> Spirits { get; set; } = [];
}