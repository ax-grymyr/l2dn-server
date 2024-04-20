using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlInstanceReenterReset
{
    [XmlAttribute("day")]
    public XmlInstanceReenterDayOfWeek DayOfWeek { get; set; }

    [XmlIgnore]
    public bool DayOfWeekSpecified { get; set; }

    [XmlAttribute("time")]
    public long TimeMinutes { get; set; }

    [XmlIgnore]
    public bool TimeMinutesSpecified { get; set; }

    [XmlAttribute("hour")]
    public int Hour { get; set; }

    [XmlAttribute("minute")]
    public int Minute { get; set; }
}