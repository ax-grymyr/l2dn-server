using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("instance")]
public class XmlInstance
{
    [XmlElement("time")]
    public XmlInstanceTime? Time { get; set; }

    [XmlElement("misc")]
    public XmlInstanceMisc? Misc { get; set; }

    [XmlElement("rates")]
    public XmlInstanceRates? Rates { get; set; }

    [XmlElement("removeBuffs")]
    public XmlInstanceRemoveBuffs? RemoveBuffs { get; set; }

    [XmlElement("locations")]
    public XmlInstanceLocations? Locations { get; set; }

    [XmlArray("parameters")]
    [XmlArrayItem("location", typeof(XmlParameterLocation))]
    [XmlArrayItem("param", typeof(XmlParameterString))]
    [XmlArrayItem("skill", typeof(XmlParameterSkill))]
    public List<XmlParameter> Parameters { get; set; } = [];

    [XmlArray("conditions")]
    [XmlArrayItem("condition")]
    public List<XmlInstanceCondition> Conditions { get; set; } = [];

    [XmlElement("reenter")]
    public XmlInstanceReenter? Reenter { get; set; }

    [XmlArray("doorlist")]
    [XmlArrayItem("door")]
    public List<XmlDoor> Doors { get; set; } = [];

    [XmlElement("spawnlist")]
    public XmlSpawn? Spawns { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("maxWorlds")]
    public int MaxWorlds { get; set; }
}