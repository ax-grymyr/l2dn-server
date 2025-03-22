using System.Xml.Serialization;
using L2Dn.GameServer.Model.Residences;

namespace L2Dn.GameServer.StaticData.Xml.ResidenceFunctions;

public sealed class XmlResidenceFunctionItem
{
    [XmlElement("function")]
    public List<XmlResidenceFunction> Functions { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("type")]
    public ResidenceFunctionType Type { get; set; }
}