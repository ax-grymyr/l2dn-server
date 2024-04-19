using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlSpawnNpcParameterMinions: XmlParameter
{
    [XmlElement("npc")]
    public List<XmlSpawnNpcParameterMinion> Npcs { get; set; } = [];
}