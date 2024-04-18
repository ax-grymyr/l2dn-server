using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlClanHallList: XmlBase
{
    [XmlElement("clanHall")]
    public List<XmlClanHall> ClanHalls { get; set; } = [];
}