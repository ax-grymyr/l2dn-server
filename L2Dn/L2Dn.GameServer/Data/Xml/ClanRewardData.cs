using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class ClanRewardData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanRewardData));
	private readonly Map<ClanRewardType, List<ClanRewardBonus>> _clanRewards = new();
	
	protected ClanRewardData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackFile("config/ClanReward.xml");
		foreach (ClanRewardType type in ClanRewardType.values())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + (_clanRewards.containsKey(type) ? _clanRewards.get(type).size() : 0) + " rewards for " + type.toString().replace("_", " ").toLowerCase() + ".");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc.getFirstChild(), IXmlReader::isNode, listNode =>
		{
			switch (listNode.getNodeName())
			{
				case "membersOnline":
				{
					parseMembersOnline(listNode);
					break;
				}
				case "huntingBonus":
				{
					parseHuntingBonus(listNode);
					break;
				}
			}
		});
	}
	
	private void parseMembersOnline(Node node)
	{
		forEach(node, IXmlReader::isNode, memberNode =>
		{
			if ("players".equalsIgnoreCase(memberNode.getNodeName()))
			{
				NamedNodeMap attrs = memberNode.getAttributes();
				int requiredAmount = parseInteger(attrs, "size");
				int level = parseInteger(attrs, "level");
				ClanRewardBonus bonus = new ClanRewardBonus(ClanRewardType.MEMBERS_ONLINE, level, requiredAmount);
				forEach(memberNode, IXmlReader::isNode, skillNode =>
				{
					if ("skill".equalsIgnoreCase(skillNode.getNodeName()))
					{
						NamedNodeMap skillAttr = skillNode.getAttributes();
						int skillId = parseInteger(skillAttr, "id");
						int skillLevel = parseInteger(skillAttr, "level");
						bonus.setSkillReward(new SkillHolder(skillId, skillLevel));
					}
				});
				_clanRewards.computeIfAbsent(bonus.getType(), key => new()).add(bonus);
			}
		});
	}
	
	private void parseHuntingBonus(Node node)
	{
		forEach(node, IXmlReader::isNode, memberNode =>
		{
			if ("hunting".equalsIgnoreCase(memberNode.getNodeName()))
			{
				NamedNodeMap attrs = memberNode.getAttributes();
				int requiredAmount = parseInteger(attrs, "points");
				int level = parseInteger(attrs, "level");
				ClanRewardBonus bonus = new ClanRewardBonus(ClanRewardType.HUNTING_MONSTERS, level, requiredAmount);
				forEach(memberNode, IXmlReader::isNode, skillNode =>
				{
					if ("skill".equalsIgnoreCase(skillNode.getNodeName()))
					{
						NamedNodeMap skillAttr = skillNode.getAttributes();
						int skillId = parseInteger(skillAttr, "id");
						int skillLevel = parseInteger(skillAttr, "level");
						bonus.setSkillReward(new SkillHolder(skillId, skillLevel));
					}
				});
				_clanRewards.computeIfAbsent(bonus.getType(), key => new()).add(bonus);
			}
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