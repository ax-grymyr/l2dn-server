using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class ClanRewardData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanRewardData));
	private ImmutableArray<ClanRewardBonus> _onlineBonuses = [];
	private ImmutableArray<ClanRewardBonus> _huntingBonuses = [];

	private ClanRewardData()
	{
		load();
	}

	public void load()
	{
		XmlClanRewardData document = LoadXmlDocument<XmlClanRewardData>(DataFileLocation.Config, "ClanReward.xml");

		// Online bonuses.
		List<ClanRewardBonus> onlineBonuses = [];
		foreach (XmlClanRewardOnlineBonus onlineBonus in document.OnlineBonuses)
		{
			if (onlineBonus.Skill is null)
			{
				_logger.Error($"{GetType().Name}: Skill is not defined for online bonus level {onlineBonus.Level}.");
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
				_logger.Error($"{GetType().Name}: Skill is not defined for hunting bonus level {huntingBonus.Level}.");
				continue;
			}

			huntingBonuses.Add(new ClanRewardBonus(ClanRewardType.HUNTING_MONSTERS, huntingBonus.Level,
				huntingBonus.Points, new SkillHolder(huntingBonus.Skill.SkillId, huntingBonus.Skill.SkillLevel)));
		}

		_onlineBonuses = onlineBonuses.OrderBy(x => x.getLevel()).ToImmutableArray();
		_huntingBonuses = huntingBonuses.OrderBy(x => x.getLevel()).ToImmutableArray();

		_logger.Info(GetType().Name + ": Loaded " + _onlineBonuses.Length + " rewards for members online.");
		_logger.Info(GetType().Name + ": Loaded " + _huntingBonuses.Length + " rewards for hunting monsters.");
	}

	public ImmutableArray<ClanRewardBonus> getClanRewardBonuses(ClanRewardType type)
	{
		return type switch
		{
			ClanRewardType.MEMBERS_ONLINE => _onlineBonuses,
			ClanRewardType.HUNTING_MONSTERS => _huntingBonuses,
			_ => [],
		};
	}

	public ClanRewardBonus? getHighestReward(ClanRewardType type)
	{
		ImmutableArray<ClanRewardBonus> bonuses = getClanRewardBonuses(type);
		return bonuses.Length == 0 ? null : bonuses[^1];
	}

	/**
	 * Gets the single instance of ClanRewardData.
	 * @return single instance of ClanRewardData
	 */
	public static ClanRewardData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ClanRewardData INSTANCE = new();
	}
}