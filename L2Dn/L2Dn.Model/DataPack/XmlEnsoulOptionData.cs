using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlEnsoulOptionData: XmlBase
{
    [XmlElement("option")]
    public List<XmlEnsoulOption> Options { get; set; } = [];
}