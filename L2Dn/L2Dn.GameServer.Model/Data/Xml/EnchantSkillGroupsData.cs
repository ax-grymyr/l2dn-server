using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// This class holds the Enchant Groups information.
/// </summary>
public sealed class EnchantSkillGroupsData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnchantSkillGroupsData));

	private int _maxEnchantLevel;

	private FrozenDictionary<int, EnchantSkillHolder> _enchantSkillHolders =
		FrozenDictionary<int, EnchantSkillHolder>.Empty;

	// TODO: separate from this class, this is dynamic data
	private readonly Map<SkillHolder, Set<int>> _enchantSkillTrees = [];

	public int MaxEnchantLevel => _maxEnchantLevel;

	/**
	 * Instantiates a new enchant groups table.
	 */
	private EnchantSkillGroupsData()
	{
		load();
	}

	public void load()
	{
		Dictionary<int, EnchantSkillHolder> enchantSkillHolders = [];

		XmlEnchantSkillGroupData document =
			LoadXmlDocument<XmlEnchantSkillGroupData>(DataFileLocation.Data, "EnchantSkillGroups.xml");

		foreach (XmlEnchantSkillGroupEnchant xmlEnchantSkillGroupEnchant in document.Enchants)
		{
			int level = xmlEnchantSkillGroupEnchant.Level;
			EnchantSkillHolder enchantSkillHolder = new(level, xmlEnchantSkillGroupEnchant.EnchantFailLevel);

			foreach (XmlEnchantSkillGroupEnchantSp xmlSp in xmlEnchantSkillGroupEnchant.Sps)
				enchantSkillHolder.addSp(xmlSp.Type, xmlSp.Amount);

			foreach (XmlEnchantSkillGroupEnchantChance xmlChance in xmlEnchantSkillGroupEnchant.Chances)
				enchantSkillHolder.addChance(xmlChance.Type, xmlChance.Value);

			foreach (XmlEnchantSkillGroupEnchantItem xmlItem in xmlEnchantSkillGroupEnchant.Items)
				enchantSkillHolder.addRequiredItem(xmlItem.Type, new ItemHolder(xmlItem.Id, xmlItem.Count));

			if (!enchantSkillHolders.TryAdd(level, enchantSkillHolder))
				_logger.Error(GetType().Name + $": Duplicated data for level {level}.");
		}

		_enchantSkillHolders = enchantSkillHolders.ToFrozenDictionary();
		_maxEnchantLevel = _enchantSkillHolders.Count;

		_logger.Info(GetType().Name + ": Loaded " + _enchantSkillHolders.Count +
			" enchant routes, max enchant set to " + _maxEnchantLevel + ".");
	}

	public void addRouteForSkill(int skillId, int level, int route)
	{
		addRouteForSkill(new SkillHolder(skillId, level), route);
	}

	public void addRouteForSkill(SkillHolder holder, int route)
	{
		_enchantSkillTrees.GetOrAdd(holder, _ => []).add(route);
	}

	public Set<int> getRouteForSkill(int skillId, int level)
	{
		return getRouteForSkill(skillId, level, 0);
	}

	public Set<int> getRouteForSkill(int skillId, int level, int subLevel)
	{
		return getRouteForSkill(new SkillHolder(skillId, level, subLevel));
	}

	public Set<int> getRouteForSkill(SkillHolder holder)
	{
		return _enchantSkillTrees.GetValueOrDefault(holder, new());
	}

	public bool isEnchantable(Skill skill)
	{
		return isEnchantable(new SkillHolder(skill.getId(), skill.getLevel()));
	}

	public bool isEnchantable(SkillHolder holder)
	{
		return _enchantSkillTrees.ContainsKey(holder);
	}

	public EnchantSkillHolder? getEnchantSkillHolder(int level)
	{
		return _enchantSkillHolders.GetValueOrDefault(level);
	}

	public static EnchantSkillGroupsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EnchantSkillGroupsData INSTANCE = new();
	}
}