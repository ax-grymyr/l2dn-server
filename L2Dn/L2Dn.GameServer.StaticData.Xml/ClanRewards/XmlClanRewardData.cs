using System.Xml.Serialization;

namespace L2Dn.GameServer.StaticData.Xml.ClanRewards;

[XmlRoot("list")]
public class XmlClanRewardData
{
    [XmlArray("membersOnline")]
    [XmlArrayItem("players")]
    public List<XmlClanRewardOnlineBonus> OnlineBonuses { get; set; } = [];

    [XmlArray("huntingBonus")]
    [XmlArrayItem("hunting")]
    public List<XmlClanRewardHuntingBonus> HuntingBonuses { get; set; } = [];
}