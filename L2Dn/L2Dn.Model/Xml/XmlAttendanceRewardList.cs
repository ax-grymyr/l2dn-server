using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlAttendanceRewardList: XmlBase
{
    [XmlElement("item")]
    public List<XmlAttendanceReward> Items { get; set; } = [];
}