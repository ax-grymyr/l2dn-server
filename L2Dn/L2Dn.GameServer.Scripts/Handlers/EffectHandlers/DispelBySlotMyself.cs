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
/// Dispel By Slot effect implementation.
/// </summary>
public sealed class DispelBySlotMyself: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _dispelAbnormals;

    public DispelBySlotMyself(StatSet @params)
    {
        string dispel = @params.getString("dispel");
        _dispelAbnormals = ParseUtil.ParseEnumSet<AbnormalType>(dispel);
    }

    public override EffectType getEffectType() => EffectType.DISPEL_BY_SLOT;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_dispelAbnormals.Count == 0)
            return;

        // The effectlist should already check if it has buff with this abnormal type or not.
        effected.getEffectList().stopEffects(info => !info.getSkill().isIrreplacableBuff() &&
            _dispelAbnormals.Contains(info.getSkill().getAbnormalType()), true, true);
    }

    public override int GetHashCode() => _dispelAbnormals.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._dispelAbnormals.GetSetComparable());
}