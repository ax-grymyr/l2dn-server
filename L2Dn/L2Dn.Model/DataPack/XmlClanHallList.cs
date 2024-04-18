using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlClanHallList: XmlBase
{
    [XmlElement("clanHall")]
    public XmlClanHall? ClanHall { get; set; }
}