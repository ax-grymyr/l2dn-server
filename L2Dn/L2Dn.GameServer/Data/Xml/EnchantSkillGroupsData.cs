using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Enchant Groups information.
 * @author Micr0
 */
public class EnchantSkillGroupsData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantSkillGroupsData));
	
	private readonly Map<int, EnchantSkillHolder> _enchantSkillHolders = new();
	private readonly Map<SkillHolder, Set<int>> _enchantSkillTrees = new();
	
	public static int MAX_ENCHANT_LEVEL;
	
	/**
	 * Instantiates a new enchant groups table.
	 */
	protected EnchantSkillGroupsData()
	{
		load();
	}
	
	public void load()
	{
		_enchantSkillHolders.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "EnchantSkillGroups.xml");
		document.Elements("list").Elements("enchant").ForEach(parseEnchant);
		
		MAX_ENCHANT_LEVEL = _enchantSkillHolders.size();
		LOGGER.Info(GetType().Name + ": Loaded " + _enchantSkillHolders.size() + " enchant routes, max enchant set to " + MAX_ENCHANT_LEVEL + ".");
	}

	private void parseEnchant(XElement element)
	{
		int level = element.GetAttributeValueAsInt32("level");
		int enchantFailLevel = element.GetAttributeValueAsInt32("enchantFailLevel");
		EnchantSkillHolder enchantSkillHolder = new EnchantSkillHolder(level, enchantFailLevel);

		element.Elements("sps").Elements("sp").ForEach(spElement =>
		{
			int amount = spElement.GetAttributeValueAsInt32("amount");
			SkillEnchantType type = spElement.Attribute("type").GetEnum<SkillEnchantType>();
			enchantSkillHolder.addSp(type, amount);
		});

		element.Elements("chances").Elements("chance").ForEach(chanceElement =>
		{
			int value = chanceElement.GetAttributeValueAsInt32("value");
			SkillEnchantType type = chanceElement.Attribute("type").GetEnum<SkillEnchantType>();
			enchantSkillHolder.addChance(type, value);
		});

		element.Elements("items").Elements("item").ForEach(chanceElement =>
		{
			int id = chanceElement.GetAttributeValueAsInt32("id");
			long count = chanceElement.GetAttributeValueAsInt64("count");
			SkillEnchantType type = chanceElement.Attribute("type").GetEnum<SkillEnchantType>();
			enchantSkillHolder.addRequiredItem(type, new ItemHolder(id, count));
		});
		
		_enchantSkillHolders.put(level, enchantSkillHolder);
	}

	public void addRouteForSkill(int skillId, int level, int route)
	{
		addRouteForSkill(new SkillHolder(skillId, level), route);
	}
	
	public void addRouteForSkill(SkillHolder holder, int route)
	{
		_enchantSkillTrees.computeIfAbsent(holder, k => new()).add(route);
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
		return _enchantSkillTrees.getOrDefault(holder, new());
	}
	
	public bool isEnchantable(Skill skill)
	{
		return isEnchantable(new SkillHolder(skill.getId(), skill.getLevel()));
	}
	
	public bool isEnchantable(SkillHolder holder)
	{
		return _enchantSkillTrees.containsKey(holder);
	}
	
	public EnchantSkillHolder getEnchantSkillHolder(int level)
	{
		return _enchantSkillHolders.getOrDefault(level, null);
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