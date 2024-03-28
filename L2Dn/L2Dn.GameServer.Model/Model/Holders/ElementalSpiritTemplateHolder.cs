using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritTemplateHolder
{
	private readonly ElementalType _type;
	private readonly byte _stage;
	private readonly int _npcId;
	private readonly int _maxCharacteristics;
	private readonly int _extractItem;

	private readonly Map<int, SpiritLevel> _levels;
	private List<ItemHolder> _itemsToEvolve;
	private List<ElementalSpiritAbsorbItemHolder> _absorbItems;

	public ElementalSpiritTemplateHolder(ElementalType type, byte stage, int npcId, int extractItem, int maxCharacteristics)
	{
		_type = type;
		_stage = stage;
		_npcId = npcId;
		_extractItem = extractItem;
		_maxCharacteristics = maxCharacteristics;
		_levels = new();
	}

	public void addLevelInfo(int level, int attack, int defense, int criticalRate, int criticalDamage,
		long maxExperience)
	{
		SpiritLevel spiritLevel = new SpiritLevel();
		spiritLevel.attack = attack;
		spiritLevel.defense = defense;
		spiritLevel.criticalRate = criticalRate;
		spiritLevel.criticalDamage = criticalDamage;
		spiritLevel.maxExperience = maxExperience;
		_levels.put(level, spiritLevel);
	}

	public void addItemToEvolve(int itemId, int count)
	{
		if (_itemsToEvolve == null)
		{
			_itemsToEvolve = new();
		}

		_itemsToEvolve.Add(new ItemHolder(itemId, count));
	}

	public ElementalType getType()
	{
		return _type;
	}

	public byte getStage()
	{
		return _stage;
	}

	public int getNpcId()
	{
		return _npcId;
	}

	public long getMaxExperienceAtLevel(int level)
	{
		SpiritLevel spiritLevel = _levels.get(level);
		return spiritLevel == null ? 0 : spiritLevel.maxExperience;
	}

	public int getMaxLevel()
	{
		return _levels.size();
	}

	public int getAttackAtLevel(int level)
	{
		return _levels.get(level).attack;
	}

	public int getDefenseAtLevel(int level)
	{
		return _levels.get(level).defense;
	}

	public int getCriticalRateAtLevel(int level)
	{
		return _levels.get(level).criticalRate;
	}

	public int getCriticalDamageAtLevel(int level)
	{
		return _levels.get(level).criticalDamage;
	}

	public int getMaxCharacteristics()
	{
		return _maxCharacteristics;
	}

	public List<ItemHolder> getItemsToEvolve()
	{
		return _itemsToEvolve == null ? new() : _itemsToEvolve;
	}

	public void addAbsorbItem(int itemId, int experience)
	{
		if (_absorbItems == null)
		{
			_absorbItems = new();
		}

		_absorbItems.Add(new ElementalSpiritAbsorbItemHolder(itemId, experience));
	}

	public List<ElementalSpiritAbsorbItemHolder> getAbsorbItems()
	{
		return _absorbItems == null ? new() : _absorbItems;
	}

	public int getExtractItem()
	{
		return _extractItem;
	}

	private class SpiritLevel
	{
		public long maxExperience;
		public int criticalDamage;
		public int criticalRate;
		public int defense;
		public int attack;
	}
}