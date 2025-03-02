using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantScrollItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("altScrollGroupId")]
    public int AltScrollGroupId { get; set; } = -1;
}