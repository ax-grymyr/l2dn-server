using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlCollectionData: XmlBase
{
    [XmlElement("collection")]
    public List<XmlCollection> Collections { get; set; } = [];
}