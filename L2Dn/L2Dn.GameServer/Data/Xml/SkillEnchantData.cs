using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class SkillEnchantData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillEnchantData));
	
	private readonly Map<int, EnchantStarHolder> _enchantStarMap = new();
	private readonly Map<int, SkillEnchantHolder> _skillEnchantMap = new();
	private readonly Map<int, Map<int, EnchantItemExpHolder>> _enchantItemMap = new();
	
	private readonly Map<int, int> _chanceEnchantMap = new();
	
	public SkillEnchantData()
	{
		load();
	}
	
	public void load()
	{
		parseDatapackFile("data/SkillEnchantData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _enchantStarMap.size() + " star levels.");
		LOGGER.Info(GetType().Name + ": Loaded " + _enchantItemMap.size() + " enchant items.");
		LOGGER.Info(GetType().Name + ": Loaded " + _skillEnchantMap.size() + " skill enchants.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode =>
		{
			forEach(listNode, "skills", skills => forEach(skills, "skill", skill =>
			{
				StatSet set = new StatSet(parseAttributes(skill));
				int id = set.getInt("id");
				_skillEnchantMap.put(id, new SkillEnchantHolder(set));
			}));
			forEach(listNode, "stars", stars => forEach(stars, "star", star =>
			{
				StatSet set = new StatSet(parseAttributes(star));
				int level = set.getInt("level");
				EnchantStarHolder starHolder = new EnchantStarHolder(set);
				_enchantStarMap.put(level, starHolder);
			}));
			forEach(listNode, "chances", itemsPoints => forEach(itemsPoints, "chance", item =>
			{
				StatSet set = new StatSet(parseAttributes(item));
				int enchantLevel = set.getInt("enchantLevel");
				int chance = set.getInt("chance");
				_chanceEnchantMap.put(enchantLevel, chance);
			}));
			forEach(listNode, "itemsPoints", itemsPoints => forEach(itemsPoints, "star", star =>
			{
				StatSet set = new StatSet(parseAttributes(star));
				int level = set.getInt("level");
				Map<int, EnchantItemExpHolder> itemMap = new();
				forEach(star, "item", item =>
				{
					StatSet statSet = new StatSet(parseAttributes(item));
					int id = statSet.getInt("id");
					itemMap.put(id, new EnchantItemExpHolder(statSet));
				});
				_enchantItemMap.put(level, itemMap);
			}));
		});
	}
	
	public EnchantStarHolder getEnchantStar(int level)
	{
		return _enchantStarMap.get(level);
	}
	
	public SkillEnchantHolder getSkillEnchant(int id)
	{
		return _skillEnchantMap.get(id);
	}
	
	public EnchantItemExpHolder getEnchantItem(int level, int id)
	{
		return _enchantItemMap.get(level).get(id);
	}
	
	public Map<int, EnchantItemExpHolder> getEnchantItem(int level)
	{
		return _enchantItemMap.get(level);
	}
	
	public int getChanceEnchantMap(Skill skill)
	{
		int enchantLevel = skill.getSubLevel() == 0 ? 1 : (skill.getSubLevel() + 1) - 1000;
		if (enchantLevel > getSkillEnchant(skill.getId()).getMaxEnchantLevel())
		{
			return 0;
		}
		
		return _chanceEnchantMap.get(enchantLevel);
	}
	
	public static SkillEnchantData getInstance()
	{
		return SkillEnchantData.SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SkillEnchantData INSTANCE = new();
	}
}