using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlSpawnList: XmlBase
{
    [XmlElement("spawn")]
    public List<XmlSpawn> Spawns { get; set; } = [];
}