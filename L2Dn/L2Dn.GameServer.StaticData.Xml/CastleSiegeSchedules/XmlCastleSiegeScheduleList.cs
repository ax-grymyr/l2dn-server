using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.CastleSiegeSchedules;

[XmlRoot("list")]
public class XmlCastleSiegeScheduleList
{
    [XmlElement("schedule")]
    public List<XmlCastleSiegeSchedule> Schedules { get; set; } = [];
}