using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlClanHall 
{
    [XmlElement("auction")]
    public XmlClanHallAuction? Auction { get; set; }

    [XmlArray("npcs")]
    [XmlArrayItem("npc")]
    public List<XmlClanHallNpc> NpcList { get; set; } = [];

    [XmlArray("doorlist")]
    [XmlArrayItem("door")]
    public List<XmlClanHallDoor> DoorList { get; set; } = [];
    
    [XmlArray("teleportList")]
    [XmlArrayItem("teleport")]
    public List<XmlClanHallTeleport> TeleportList { get; set; } = [];
    
    [XmlElement("ownerRestartPoint")]
    public XmlLocation? OwnerRestartPoint { get; set; }

    [XmlElement("banishPoint")]
    public XmlLocation? BanishPoint { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
    
    [XmlAttribute("grade")]
    public ClanHallGrade Grade { get; set; }
    
    [XmlIgnore]
    public bool GradeSpecified { get; set; }

    [XmlAttribute("type")]
    public ClanHallType Type { get; set; }

    [XmlIgnore]
    public bool TypeSpecified { get; set; }
}