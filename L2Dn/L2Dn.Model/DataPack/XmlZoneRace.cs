using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlZoneRace
{
    [XmlAttribute("name")]
    public Race Race { get; set; }

    [XmlAttribute("point")]
    public string Point { get; set; } = string.Empty;
}