using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.Teleporters;

public class XmlTeleporterNpcInner
{
    [XmlAttribute("id")]
    public int Id { get; set; }
}