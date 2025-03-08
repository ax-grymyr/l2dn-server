using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlAttendanceRewardList
{
    [XmlElement("item")]
    public List<XmlAttendanceReward> Items { get; set; } = [];
}