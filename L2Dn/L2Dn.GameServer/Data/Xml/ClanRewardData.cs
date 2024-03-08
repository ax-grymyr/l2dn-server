using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class ClanRewardData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanRewardData));
	private readonly Map<ClanRewardType, List<ClanRewardBonus>> _clanRewards = new();
	
	protected ClanRewardData()
	{
		load();
	}
	
	public void load()
	{
		XDocument document = LoadXmlDocument(DataFileLocation.Config, "ClanReward.xml");
		document.Elements("list").Elements("membersOnline").ForEach(parseMembersOnline);
		document.Elements("list").Elements("huntingBonus").ForEach(parseHuntingBonus);

		foreach (ClanRewardType type in Enum.GetValues<ClanRewardType>())
		{
			LOGGER.Info(GetType().Name + ": Loaded " +
			            (_clanRewards.containsKey(type) ? _clanRewards.get(type).size() : 0) + " rewards for " +
			            type.ToString().Replace("_", " ").toLowerCase() + ".");
		}
	}
	
	private void parseMembersOnline(XElement node)
	{
		node.Elements("players").ForEach(el =>
		{
			int requiredAmount = el.GetAttributeValueAsInt32("size");
			int level = el.GetAttributeValueAsInt32("level");
			ClanRewardBonus bonus = new ClanRewardBonus(ClanRewardType.MEMBERS_ONLINE, level, requiredAmount);
			el.Elements("skill").ForEach(e =>
			{
				int skillId = e.GetAttributeValueAsInt32("id");
				int skillLevel = e.GetAttributeValueAsInt32("level");
				bonus.setSkillReward(new SkillHolder(skillId, skillLevel));
			});
			
			_clanRewards.computeIfAbsent(bonus.getType(), key => new()).add(bonus);
		});
	}
	
	private void parseHuntingBonus(XElement node)
	{
		node.Elements("hunting").ForEach(el =>
		{
			int requiredAmount = el.GetAttributeValueAsInt32("points");
			int level = el.GetAttributeValueAsInt32("level");
			ClanRewardBonus bonus = new ClanRewardBonus(ClanRewardType.HUNTING_MONSTERS, level, requiredAmount);
			el.Elements("skill").ForEach(e =>
			{
				int skillId = e.GetAttributeValueAsInt32("id");
				int skillLevel = e.GetAttributeValueAsInt32("level");
				bonus.setSkillReward(new SkillHolder(skillId, skillLevel));
			});
			
			_clanRewards.computeIfAbsent(bonus.getType(), key => new()).add(bonus);
		});
	}
	
	public List<ClanRewardBonus> getClanRewardBonuses(ClanRewardType type)
	{
		return _clanRewards.get(type);
	}
	
	public ClanRewardBonus getHighestReward(ClanRewardType type)
	{
		ClanRewardBonus selectedBonus = null;
		foreach (ClanRewardBonus currentBonus in _clanRewards.get(type))
		{
			if ((selectedBonus == null) || (selectedBonus.getLevel() < currentBonus.getLevel()))
			{
				selectedBonus = currentBonus;
			}
		}
		return selectedBonus;
	}
	
	public ICollection<List<ClanRewardBonus>> getClanRewardBonuses()
	{
		return _clanRewards.values();
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