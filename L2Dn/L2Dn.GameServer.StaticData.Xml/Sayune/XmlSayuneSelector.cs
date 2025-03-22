using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Sayune;

public sealed class XmlSayuneSelector: XmlSayuneLocation
{
    [XmlElement("choice")]
    public List<XmlSayuneSelectorChoice> Choices { get; set; } = [];
}