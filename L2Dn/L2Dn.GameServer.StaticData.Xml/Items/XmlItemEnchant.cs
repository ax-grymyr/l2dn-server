using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemEnchant
{
    [XmlAttribute("order")]
    public int Order { get; set; }

    [XmlIgnore]
    public bool OrderSpecified { get; set; }

    [XmlAttribute("stat")]
    public Stat Stat { get; set; }

    [XmlAttribute("val")]
    public int Value { get; set; }
}