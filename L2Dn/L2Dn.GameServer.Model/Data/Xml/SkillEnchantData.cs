using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class SkillEnchantData: DataReaderBase
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
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "SkillEnchantData.xml");
		document.Elements("list").Elements("skills").Elements("skill").ForEach(parseSkillElement);
		document.Elements("list").Elements("stars").Elements("star").ForEach(parseStarElement);
		document.Elements("list").Elements("chances").Elements("chance").ForEach(parseChanceElement);
		document.Elements("list").Elements("itemsPoints").Elements("star").ForEach(parseItemPointStarElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _enchantStarMap.Count + " star levels.");
		LOGGER.Info(GetType().Name + ": Loaded " + _enchantItemMap.Count + " enchant items.");
		LOGGER.Info(GetType().Name + ": Loaded " + _skillEnchantMap.Count + " skill enchants.");
	}

	private void parseSkillElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		int starLevel = element.GetAttributeValueAsInt32("starLevel");
		int maxEnchantLevel = element.GetAttributeValueAsInt32("maxEnchantLevel");
		_skillEnchantMap.put(id, new SkillEnchantHolder(id, starLevel, maxEnchantLevel));
	}

	private void parseStarElement(XElement element)
	{
		int level = element.GetAttributeValueAsInt32("level");
		int expMax = element.GetAttributeValueAsInt32("expMax");
		int expOnFail = element.GetAttributeValueAsInt32("expOnFail");
		long feeAdena = element.GetAttributeValueAsInt64("feeAdena");
		EnchantStarHolder starHolder = new EnchantStarHolder(level, expMax, expOnFail, feeAdena);
		_enchantStarMap.put(level, starHolder);
	}

	private void parseChanceElement(XElement element)
	{
		int enchantLevel = element.GetAttributeValueAsInt32("enchantLevel");
		int chance = element.GetAttributeValueAsInt32("chance");
		_chanceEnchantMap.put(enchantLevel, chance);
	}

	private void parseItemPointStarElement(XElement element)
	{
		int level = element.GetAttributeValueAsInt32("level");
		Map<int, EnchantItemExpHolder> itemMap = new();
		element.Elements("item").ForEach(el =>
		{
			int id = el.GetAttributeValueAsInt32("id");
			int exp = el.Attribute("exp").GetInt32(1);
			int starLevel = el.Attribute("starLevel").GetInt32(1);
			itemMap.put(id, new EnchantItemExpHolder(id, exp, starLevel));
		});

		_enchantItemMap.put(level, itemMap);
	}

	public EnchantStarHolder? getEnchantStar(int level)
	{
		return _enchantStarMap.get(level);
	}

	public SkillEnchantHolder? getSkillEnchant(int id)
	{
		return _skillEnchantMap.get(id);
	}

	public EnchantItemExpHolder? getEnchantItem(int level, int id)
	{
		return _enchantItemMap.get(level)?.get(id);
	}

	public Map<int, EnchantItemExpHolder>? getEnchantItem(int level)
	{
		return _enchantItemMap.get(level);
	}

	public int getChanceEnchantMap(Skill skill)
	{
		int enchantLevel = skill.SubLevel == 0 ? 1 : skill.SubLevel + 1 - 1000;
		if (enchantLevel > getSkillEnchant(skill.Id)?.getMaxEnchantLevel())
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