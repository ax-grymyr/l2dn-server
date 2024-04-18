using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlCastleList
{
    [XmlElement("castle")]
    public List<XmlCastle> Castles { get; set; } = [];
}