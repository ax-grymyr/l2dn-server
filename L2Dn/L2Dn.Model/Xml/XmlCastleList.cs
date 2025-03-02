using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlCastleList: XmlBase
{
    [XmlElement("castle")]
    public List<XmlCastle> Castles { get; set; } = [];
}