using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.CastleSiegeSchedules;

public class XmlCastleSiegeSchedule
{
    [XmlAttribute("castleId")]
    public int CastleId { get; set; }

    [XmlAttribute("castleName")]
    public string CastleName { get; set; } = string.Empty;

    [XmlAttribute("siegeEnabled")]
    public bool SiegeEnabled { get; set; }

    [XmlAttribute("day")]
    public string Day { get; set; } = string.Empty;

    [XmlAttribute("hour")]
    public int Hour { get; set; }

    [XmlIgnore]
    public bool HourSpecified { get; set; }

    [XmlAttribute("maxConcurrent")]
    public int MaxConcurrent { get; set; }

    [XmlIgnore]
    public bool MaxConcurrentSpecified { get; set; }
}