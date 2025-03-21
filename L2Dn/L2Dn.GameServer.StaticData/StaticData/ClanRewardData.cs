using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData.Xml.ClanRewards;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class ClanRewardData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanRewardData));
    private ImmutableArray<ClanRewardBonus> _onlineBonuses = [];
    private ImmutableArray<ClanRewardBonus> _huntingBonuses = [];

    private ClanRewardData()
    {
    }

    public static ClanRewardData Instance { get; } = new();

    public void Load()
    {
        XmlClanRewardData document = XmlLoader.LoadConfigXmlDocument<XmlClanRewardData>("ClanReward.xml");

        // Online bonuses.
        List<ClanRewardBonus> onlineBonuses = [];
        foreach (XmlClanRewardOnlineBonus onlineBonus in document.OnlineBonuses)
        {
            if (onlineBonus.Skill is null)
            {
                _logger.Error(
                    $"{nameof(ClanRewardData)}: Skill is not defined for online bonus level {onlineBonus.Level}.");

                continue;
            }

            onlineBonuses.Add(new ClanRewardBonus(ClanRewardType.MEMBERS_ONLINE, onlineBonus.Level, onlineBonus.Count,
                new SkillHolder(onlineBonus.Skill.SkillId, onlineBonus.Skill.SkillLevel)));
        }

        // Hunting bonuses.
        List<ClanRewardBonus> huntingBonuses = [];
        foreach (XmlClanRewardHuntingBonus huntingBonus in document.HuntingBonuses)
        {
            if (huntingBonus.Skill is null)
            {
                _logger.Error(
                    $"{nameof(ClanRewardData)}: Skill is not defined for hunting bonus level {huntingBonus.Level}.");

                continue;
            }

            huntingBonuses.Add(new ClanRewardBonus(ClanRewardType.HUNTING_MONSTERS, huntingBonus.Level,
                huntingBonus.Points, new SkillHolder(huntingBonus.Skill.SkillId, huntingBonus.Skill.SkillLevel)));
        }

        _onlineBonuses = onlineBonuses.OrderBy(x => x.Level).ToImmutableArray();
        _huntingBonuses = huntingBonuses.OrderBy(x => x.Level).ToImmutableArray();

        _logger.Info($"{nameof(ClanRewardData)}: Loaded {_onlineBonuses.Length} rewards for members online.");
        _logger.Info($"{nameof(ClanRewardData)}: Loaded {_huntingBonuses.Length} rewards for hunting monsters.");
    }

    public ImmutableArray<ClanRewardBonus> GetClanRewardBonuses(ClanRewardType type) =>
        type switch
        {
            ClanRewardType.MEMBERS_ONLINE => _onlineBonuses,
            ClanRewardType.HUNTING_MONSTERS => _huntingBonuses,
            _ => [],
        };

    public ClanRewardBonus? GetHighestReward(ClanRewardType type)
    {
        ImmutableArray<ClanRewardBonus> bonuses = GetClanRewardBonuses(type);
        return bonuses.Length == 0 ? null : bonuses[^1];
    }
}