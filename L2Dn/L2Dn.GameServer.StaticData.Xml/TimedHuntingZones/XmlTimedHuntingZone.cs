using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.TimedHuntingZones;

public class XmlTimedHuntingZone
{
    [XmlElement("enterLocation")]
    public List<string> EnterLocations { get; set; } = [];

    [XmlElement("exitLocation")]
    public string ExitLocation { get; set; } = string.Empty;

    [XmlElement("initialTime")]
    public int InitialTimeSeconds { get; set; }

    [XmlElement("resetDelay")]
    public int ResetDelaySeconds { get; set; }

    [XmlIgnore]
    public bool ResetDelaySecondsSpecified { get; set; }

    [XmlElement("maxAddedTime")]
    public int MaxAddedTimeSeconds { get; set; }

    [XmlElement("remainRefillTime")]
    public int RemainRefillTimeSeconds { get; set; }

    [XmlIgnore]
    public bool RemainRefillTimeSecondsSpecified { get; set; }

    [XmlElement("refillTimeMax")]
    public int RefillTimeMaxSeconds { get; set; }

    [XmlIgnore]
    public bool RefillTimeMaxSecondsSpecified { get; set; }

    [XmlElement("entryItemId")]
    public int EntryItemId { get; set; }

    [XmlIgnore]
    public bool EntryItemIdSpecified { get; set; }

    [XmlElement("entryFee")]
    public int EntryFee { get; set; }

    [XmlElement("minLevel")]
    public int MinLevel { get; set; }

    [XmlElement("maxLevel")]
    public int MaxLevel { get; set; }

    [XmlElement("instanceId")]
    public int InstanceId { get; set; }

    [XmlIgnore]
    public bool InstanceIdSpecified { get; set; }

    [XmlElement("soloInstance")]
    public bool SoloInstance { get; set; }

    [XmlIgnore]
    public bool SoloInstanceSpecified { get; set; }

    [XmlElement("pvpZone")]
    public bool PvpZone { get; set; }

    [XmlIgnore]
    public bool PvpZoneSpecified { get; set; }

    [XmlElement("noPvpZone")]
    public bool NoPvpZone { get; set; }

    [XmlIgnore]
    public bool NoPvpZoneSpecified { get; set; }

    [XmlElement("weekly")]
    public bool Weekly { get; set; }

    [XmlIgnore]
    public bool WeeklySpecified { get; set; }

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlIgnore]
    public bool IdSpecified { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}