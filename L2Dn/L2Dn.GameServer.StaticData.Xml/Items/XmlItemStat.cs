using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.StaticData.Xml.Items;

public sealed class XmlItemStat
{
    [XmlAttribute("type")]
    public Stat Type { get; set; }

    [XmlText]
    public double Value { get; set; }
}