using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public abstract class XmlSpawnTerritoryBase
{
    [XmlElement("node")]
    public List<XmlNode2D> Nodes { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("minZ")]
    public int MinZ { get; set; }

    [XmlAttribute("maxZ")]
    public int MaxZ { get; set; }

    [XmlAttribute("rad")]
    public int Radius { get; set; }

    [XmlAttribute("shape")]
    public XmlSpawnTerritoryShape Shape { get; set; }

    [XmlIgnore]
    public bool ShapeSpecified { get; set; }
}