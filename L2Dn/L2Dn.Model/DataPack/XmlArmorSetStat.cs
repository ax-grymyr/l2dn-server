using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlArmorSetStat 
{
    [XmlAttribute("type")]
    public BaseStat Stat { get; set; }

    [XmlAttribute("val")]
    public double Value { get; set; }
}