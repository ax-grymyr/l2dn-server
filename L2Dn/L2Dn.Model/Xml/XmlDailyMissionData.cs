using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.Xml;

[XmlRoot("list")]
public class XmlDailyMissionData: XmlBase
{
    [XmlElement("reward")]
    public List<XmlDailyMission> DailyMissions { get; set; } = [];
}

public class XmlDailyMission
{
    [XmlElement("classId")]
    public List<int> ClassIds { get; set; } = [];

    [XmlElement("handler")]
    public XmlDailyMissionHandler? Handler { get; set; }

    [XmlArray("items")]
    [XmlArrayItem("item")]
    public List<XmlDailyMissionReward> Rewards { get; set; } = [];

    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("reward_id")]
    public int RewardId { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlAttribute("requiredCompletion")]
    public int RequiredCompletion { get; set; }

    [XmlAttribute("dailyReset")]
    public bool DailyReset { get; set; } = true;

    [XmlAttribute("isOneTime")]
    public bool IsOneTime { get; set; } = true;

    [XmlAttribute("isMainClassOnly")]
    public bool IsMainClassOnly { get; set; } = true;

    [XmlAttribute("isDualClassOnly")]
    public bool IsDualClassOnly { get; set; }

    [XmlAttribute("isDisplayedWhenNotAvailable")]
    public bool IsDisplayedWhenNotAvailable { get; set; } = true;

    [XmlAttribute("duration")]
    public MissionResetType Duration { get; set; } = MissionResetType.DAY;
}

public class XmlDailyMissionHandler
{
    [XmlElement("param")]
    public List<XmlDailyMissionHandlerParam> Parameters { get; set; } = [];

    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;
}

public class XmlDailyMissionHandlerParam
{
    [XmlAttribute("name")]
    public string Name { get; set; } = string.Empty;

    [XmlText]
    public string Value { get; set; } = string.Empty;
}

public class XmlDailyMissionReward
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("count")]
    public int Count { get; set; } = 1;
}