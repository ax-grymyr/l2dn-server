using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlElementalSpiritItemToEvolve
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; } = 1;
}