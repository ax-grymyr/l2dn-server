using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class HennaPatternPotentialData: DataReaderBase
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
		_potenFees.Clear();
		_potenExpTable.Clear();
		_potentials.Clear();
		_enchancedReset.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/hennaPatternPotential.xml");
		document.Elements("list").ForEach(element =>
		{
			element.Elements("enchantFees").Elements("fee").ForEach(parseFeeElement);
			element.Elements("resetCount").Elements("reset").ForEach(parseResetElement);
			element.Elements("experiencePoints").Elements("hiddenPower").ForEach(parseHiddenPowerElement);
			element.Elements("hiddenPotentials").Elements("poten").ForEach(parseHiddenPotenElement);
		});

		LOGGER.Info(GetType().Name + ": Loaded " + _potenFees.Count + " dye pattern fee data.");

	}

	private void parseFeeElement(XElement element)
	{
		int step = element.GetAttributeValueAsInt32("step");
		int dailyCount = 0;
		Map<int, double> enchantExp = new();
		List<ItemHolder> items = new();

		element.Elements("requiredItem").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			long itemCount = el.Attribute("count").GetInt64(1);
			items.Add(new ItemHolder(itemId, itemCount));
		});

		element.Elements("dailyCount").ForEach(el =>
		{
			int value = (int)el;
			dailyCount = value;
		});

		element.Elements("enchantExp").ForEach(el =>
		{
			int count = el.GetAttributeValueAsInt32("count");
			double chance = el.GetAttributeValueAsDouble("chance");
			enchantExp.put(count, chance);
		});

		_potenFees.put(step, new DyePotentialFee(step, items, dailyCount, enchantExp));
	}

	private void parseResetElement(XElement element)
	{
		int itemId = element.GetAttributeValueAsInt32("itemid");
		int itemCount = element.GetAttributeValueAsInt32("count");
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			LOGGER.Error(GetType().Name + ": Item with id " + itemId + " does not exist.");
		}
		else
		{
			_enchancedReset.Add(new ItemHolder(itemId, itemCount));
		}
	}

	private void parseHiddenPowerElement(XElement element)
	{
		int level = element.GetAttributeValueAsInt32("level");
		int exp = element.GetAttributeValueAsInt32("exp");
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
		int id = element.GetAttributeValueAsInt32("id");
		int slotId = element.GetAttributeValueAsInt32("slotId");
		int maxSkillLevel = element.GetAttributeValueAsInt32("maxSkillLevel");
		int skillId = element.GetAttributeValueAsInt32("skillId");
		_potentials.put(id, new DyePotential(id, slotId, skillId, maxSkillLevel));
	}

	public DyePotentialFee? getFee(int step)
	{
		return _potenFees.get(step);
	}

	public int getMaxPotenEnchantStep()
	{
		return _potenFees.Count;
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

	public DyePotential? getPotential(int potenId)
	{
		return _potentials.get(potenId);
	}

	public Skill? getPotentialSkill(int potenId, int slotId, int level)
	{
		DyePotential? potential = _potentials.get(potenId);
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
		foreach (DyePotential potential in _potentials.Values)
		{
			if (potential.getSlotId() == slotId)
			{
				skillIds.Add(potential.getSkillId());
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