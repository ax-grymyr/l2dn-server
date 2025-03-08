using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCastleList
{
    [XmlElement("castle")]
    public List<XmlCastle> Castles { get; set; } = [];
}