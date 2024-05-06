using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlElementalSpiritAbsorbItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("experience")]
    public int Experience { get; set; }
}