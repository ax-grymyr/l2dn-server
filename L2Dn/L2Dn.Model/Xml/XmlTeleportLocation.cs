﻿using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlTeleportLocation: XmlLocation
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
    
    [XmlIgnore]
    public bool NameSpecified { get; set; }

    [XmlAttribute("npcStringId")]
    public int NpcStringId { get; set; }
    
    [XmlIgnore]
    public bool NpcStringIdSpecified { get; set; }
    
    [XmlAttribute("questZoneId")]
    public int QuestZoneId { get; set; }
    
    [XmlIgnore]
    public bool QuestZoneIdSpecified { get; set; }
    
    [XmlAttribute("feeId")]
    public int FeeItemId { get; set; }
    
    [XmlIgnore]
    public bool FeeItemIdSpecified { get; set; }
    
    [XmlAttribute("feeCount")]
    public long FeeCount { get; set; }
    
    [XmlIgnore]
    public bool FeeCountSpecified { get; set; }

    [XmlAttribute("castleId")]
    public string CastleId { get; set; } = string.Empty;
    
    [XmlIgnore]
    public bool CastleIdSpecified { get; set; }
}