using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Enchant Groups information.
 * @author Micr0
 */
public class EnchantSkillGroupsData
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
		parseDatapackFile("data/EnchantSkillGroups.xml");
		MAX_ENCHANT_LEVEL = _enchantSkillHolders.size();
		LOGGER.Info(GetType().Name + ": Loaded " + _enchantSkillHolders.size() + " enchant routes, max enchant set to " + MAX_ENCHANT_LEVEL + ".");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "enchant", enchantNode =>
		{
			EnchantSkillHolder enchantSkillHolder = new EnchantSkillHolder(new StatSet(parseAttributes(enchantNode)));
			forEach(enchantNode, "sps", spsNode => forEach(spsNode, "sp", spNode => enchantSkillHolder.addSp(parseEnum(spNode.getAttributes(), SkillEnchantType.class, "type"), parseInteger(spNode.getAttributes(), "amount"))));
			forEach(enchantNode, "chances", chancesNode => forEach(chancesNode, "chance", chanceNode => enchantSkillHolder.addChance(parseEnum(chanceNode.getAttributes(), SkillEnchantType.class, "type"), parseInteger(chanceNode.getAttributes(), "value"))));
			forEach(enchantNode, "items", itemsNode => forEach(itemsNode, "item", itemNode => enchantSkillHolder.addRequiredItem(parseEnum(itemNode.getAttributes(), SkillEnchantType.class, "type"), new ItemHolder(new StatSet(parseAttributes(itemNode))))));
			_enchantSkillHolders.put(parseInteger(enchantNode.getAttributes(), "level"), enchantSkillHolder);
		}));
	}
	
	public void addRouteForSkill(int skillId, int level, int route)
	{
		addRouteForSkill(new SkillHolder(skillId, level), route);
	}
	
	public void addRouteForSkill(SkillHolder holder, int route)
	{
		_enchantSkillTrees.computeIfAbsent(holder, k => new HashSet<>()).add(route);
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