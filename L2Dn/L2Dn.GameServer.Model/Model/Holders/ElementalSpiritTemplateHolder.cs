using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.Model.Enums;

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
	private readonly FrozenDictionary<int, ElementalSpiritLevel> _levels;
	private readonly ImmutableArray<ItemHolder> _itemsToEvolve;
	private readonly ImmutableArray<ElementalSpiritAbsorbItemHolder> _absorbItems;

	public ElementalSpiritTemplateHolder(ElementalType type, byte stage, int npcId, int extractItem,
		int maxCharacteristics, ImmutableArray<ElementalSpiritLevel> levels, ImmutableArray<ItemHolder> itemsToEvolve,
		ImmutableArray<ElementalSpiritAbsorbItemHolder> absorbItems)
	{
		_type = type;
		_stage = stage;
		_npcId = npcId;
		_extractItem = extractItem;
		_maxCharacteristics = maxCharacteristics;
		_levels = levels.ToFrozenDictionary(l => l.Level);
		_itemsToEvolve = itemsToEvolve;
		_absorbItems = absorbItems;
	}

	public ElementalType getType() => _type;
	public byte getStage() => _stage;
	public int getNpcId() => _npcId;
	public long getMaxExperienceAtLevel(int level) => _levels.GetValueOrDefault(level)?.MaxExperience ?? 0;
	public int getMaxLevel() => _levels.Count;
	public int getAttackAtLevel(int level) => _levels.GetValueOrDefault(level)?.Attack ?? 0;
	public int getDefenseAtLevel(int level) => _levels.GetValueOrDefault(level)?.Defense ?? 0;
	public int getCriticalRateAtLevel(int level) => _levels.GetValueOrDefault(level)?.CriticalRate ?? 0;
	public int getCriticalDamageAtLevel(int level) => _levels.GetValueOrDefault(level)?.CriticalDamage ?? 0;
	public int getMaxCharacteristics() => _maxCharacteristics;
	public ImmutableArray<ItemHolder> getItemsToEvolve() => _itemsToEvolve;
	public ImmutableArray<ElementalSpiritAbsorbItemHolder> getAbsorbItems() => _absorbItems;
	public int getExtractItem() => _extractItem;
}