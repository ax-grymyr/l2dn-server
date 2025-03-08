using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlClanHallList
{
    [XmlElement("clanHall")]
    public List<XmlClanHall> ClanHalls { get; set; } = [];
}