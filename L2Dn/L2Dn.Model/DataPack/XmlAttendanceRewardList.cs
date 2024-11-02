using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

[XmlRoot("list")]
public class XmlAttendanceRewardList: XmlBase
{
    [XmlElement("item")]
    public List<XmlAttendanceReward> Items { get; set; } = [];
}