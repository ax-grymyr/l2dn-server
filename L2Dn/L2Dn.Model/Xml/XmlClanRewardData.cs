using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlClanRewardData: XmlBase
{
    [XmlArray("membersOnline")]
    [XmlArrayItem("players")]
    public List<XmlClanRewardOnlineBonus> OnlineBonuses { get; set; } = [];

    [XmlArray("huntingBonus")]
    [XmlArrayItem("hunting")]
    public List<XmlClanRewardHuntingBonus> HuntingBonuses { get; set; } = [];
}