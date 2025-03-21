using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.Vip;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class VipData
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(VipData));
    private FrozenDictionary<int, VipInfo> _vipTiers = FrozenDictionary<int, VipInfo>.Empty;

    private VipData()
    {
    }

    public static VipData Instance { get; } = new();

    public void Load()
    {
        if (!Config.VipSystem.VIP_SYSTEM_ENABLED)
            return;

        _vipTiers = XmlLoader.LoadXmlDocument<XmlVipData>("Vip.xml").Tiers.Select(tier =>
            new VipInfo(tier.Tier, tier.PointsRequired, tier.PointsLose,
                tier.Bonuses.Select(b => new VipBonusInfo(b.SkillId, b.SilverChance, b.GoldChance)).
                    ToImmutableArray())).ToFrozenDictionary(t => t.Tier);

        LOGGER.Info(nameof(VipData) + ": Loaded " + _vipTiers.Count + " vips.");
    }

    public FrozenDictionary<int, VipInfo> VipTiers => _vipTiers;
}