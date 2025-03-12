using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block Buff Slot effect implementation.
/// </summary>
public sealed class BlockAbnormalSlot: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _blockAbnormalSlots;

    public BlockAbnormalSlot(StatSet @params)
    {
        string slot = @params.getString("slot");
        _blockAbnormalSlots = ParseUtil.ParseEnumSet<AbnormalType>(slot);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getEffectList().addBlockedAbnormalTypes(_blockAbnormalSlots);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getEffectList().removeBlockedAbnormalTypes(_blockAbnormalSlots);
    }

    public override int GetHashCode() => _blockAbnormalSlots.GetSetHashCode();

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => x._blockAbnormalSlots.GetSetComparable());
}