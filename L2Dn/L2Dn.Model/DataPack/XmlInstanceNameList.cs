using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlInstanceNameList: XmlBase
{
    [XmlElement("instance")]
    public List<XmlInstanceName> Instances { get; set; } = [];
}