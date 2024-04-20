using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlOptionData: XmlBase
{
    [XmlElement("option")]
    public List<XmlOption> Options { get; set; } = [];
}