using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

public class XmlElementalAttributeItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("elemental")]
    public AttributeType Elemental { get; set; }

    [XmlAttribute("type")]
    public ElementalItemType Type { get; set; }

    [XmlAttribute("power")]
    public short Power { get; set; }
}