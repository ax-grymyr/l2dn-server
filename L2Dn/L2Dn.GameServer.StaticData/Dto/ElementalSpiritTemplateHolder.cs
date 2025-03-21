using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Dto;

public sealed class ElementalSpiritTemplateHolder
{
    private readonly FrozenDictionary<int, ElementalSpiritLevel> _levels;

    public ElementalSpiritTemplateHolder(ElementalType type, byte stage, int npcId, int extractItem,
        int maxCharacteristics, ImmutableArray<ElementalSpiritLevel> levels, ImmutableArray<ItemHolder> itemsToEvolve,
        ImmutableArray<ElementalSpiritAbsorbItemHolder> absorbItems)
    {
        Type = type;
        Stage = stage;
        NpcId = npcId;
        ExtractItemId = extractItem;
        MaxCharacteristics = maxCharacteristics;
        _levels = levels.ToFrozenDictionary(l => l.Level);
        ItemsToEvolve = itemsToEvolve;
        AbsorbItems = absorbItems;
    }

    public ElementalType Type { get; }

    public byte Stage { get; }

    public int NpcId { get; }

    public long GetMaxExperienceAtLevel(int level) => _levels.GetValueOrDefault(level)?.MaxExperience ?? 0;

    public int MaxLevel => _levels.Count;

    public int GetAttackAtLevel(int level) => _levels.GetValueOrDefault(level)?.Attack ?? 0;
    public int GetDefenseAtLevel(int level) => _levels.GetValueOrDefault(level)?.Defense ?? 0;
    public int GetCriticalRateAtLevel(int level) => _levels.GetValueOrDefault(level)?.CriticalRate ?? 0;
    public int GetCriticalDamageAtLevel(int level) => _levels.GetValueOrDefault(level)?.CriticalDamage ?? 0;

    public int MaxCharacteristics { get; }

    public ImmutableArray<ItemHolder> ItemsToEvolve { get; }

    public ImmutableArray<ElementalSpiritAbsorbItemHolder> AbsorbItems { get; }

    public int ExtractItemId { get; }
}