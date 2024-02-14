using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class HennaPatternPotentialData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HennaPatternPotentialData));
	
	private readonly Map<int, int> _potenExpTable = new();
	private readonly Map<int, DyePotentialFee> _potenFees = new();
	private readonly Map<int, DyePotential> _potentials = new();
	private readonly List<ItemHolder> _enchancedReset = new();
	
	private int MAX_POTEN_LEVEL = 0;
	private int MAX_POTEN_EXP = 0;
	
	protected HennaPatternPotentialData()
	{
		load();
	}
	
	public void load()
	{
		_potenFees.clear();
		_potenExpTable.clear();
		_potentials.clear();
		_enchancedReset.Clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/stats/hennaPatternPotential.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").ForEach(element =>
		{
			element.Elements("enchantFees").Elements("fee").ForEach(parseFeeElement);
			element.Elements("resetCount").Elements("reset").ForEach(parseResetElement);
			element.Elements("experiencePoints").Elements("hiddenPower").ForEach(parseHiddenPowerElement);
			element.Elements("hiddenPotentials").Elements("poten").ForEach(parseHiddenPotenElement);
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _potenFees.size() + " dye pattern fee data.");
		
	}

	private void parseFeeElement(XElement element)
	{
		int step = element.Attribute("step").GetInt32();
		int dailyCount = 0;
		Map<int, double> enchantExp = new();
		List<ItemHolder> items = new();
		
		element.Elements("requiredItem").ForEach(el =>
		{
			int itemId = el.Attribute("id").GetInt32();
			long itemCount = el.Attribute("count").GetInt64(1);
			items.add(new ItemHolder(itemId, itemCount));
		});
		
		element.Elements("dailyCount").ForEach(el =>
		{
			int value = (int)el;
			dailyCount = value;
		});
		
		element.Elements("enchantExp").ForEach(el =>
		{
			int count = el.Attribute("count").GetInt32();
			double chance = el.Attribute("chance").GetDouble();
			enchantExp.put(count, chance);
		});

		_potenFees.put(step, new DyePotentialFee(step, items, dailyCount, enchantExp));
	}

	private void parseResetElement(XElement element)
	{
		int itemId = element.Attribute("itemid").GetInt32();
		int itemCount = element.Attribute("count").GetInt32();
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			LOGGER.Error(GetType().Name + ": Item with id " + itemId + " does not exist.");
		}
		else
		{
			_enchancedReset.add(new ItemHolder(itemId, itemCount));
		}
	}

	private void parseHiddenPowerElement(XElement element)
	{
		int level = element.Attribute("level").GetInt32();
		int exp = element.Attribute("exp").GetInt32();
		_potenExpTable.put(level, exp);
		if (MAX_POTEN_LEVEL < level)
		{
			MAX_POTEN_LEVEL = level;
		}
		if (MAX_POTEN_EXP < exp)
		{
			MAX_POTEN_EXP = exp;
		}
	}

	private void parseHiddenPotenElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		int slotId = element.Attribute("slotId").GetInt32();
		int maxSkillLevel = element.Attribute("maxSkillLevel").GetInt32();
		int skillId = element.Attribute("skillId").GetInt32();
		_potentials.put(id, new DyePotential(id, slotId, skillId, maxSkillLevel));
	}
	
	public DyePotentialFee getFee(int step)
	{
		return _potenFees.get(step);
	}
	
	public int getMaxPotenEnchantStep()
	{
		return _potenFees.size();
	}
	
	public List<ItemHolder> getEnchantReset()
	{
		return _enchancedReset;
	}
	
	public int getExpForLevel(int level)
	{
		return _potenExpTable.get(level);
	}
	
	public int getMaxPotenLevel()
	{
		return MAX_POTEN_LEVEL;
	}
	
	public int getMaxPotenExp()
	{
		return MAX_POTEN_EXP;
	}
	
	public DyePotential getPotential(int potenId)
	{
		return _potentials.get(potenId);
	}
	
	public Skill getPotentialSkill(int potenId, int slotId, int level)
	{
		DyePotential potential = _potentials.get(potenId);
		if (potential == null)
		{
			return null;
		}
		if (potential.getSlotId() == slotId)
		{
			return potential.getSkill(level);
		}
		return null;
	}
	
	public ICollection<int> getSkillIdsBySlotId(int slotId)
	{
		List<int> skillIds = new();
		foreach (DyePotential potential in _potentials.values())
		{
			if (potential.getSlotId() == slotId)
			{
				skillIds.add(potential.getSkillId());
			}
		}
		return skillIds;
	}
	
	public static HennaPatternPotentialData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly HennaPatternPotentialData INSTANCE = new();
	}
}