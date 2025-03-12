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
/// Dispel By Slot Probability effect implementation.
/// </summary>
public sealed class DispelBySlotProbability: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _dispelAbnormals;
    private readonly int _rate;

    public DispelBySlotProbability(StatSet @params)
    {
        string dispelEffects = @params.getString("dispel");
        _dispelAbnormals = ParseUtil.ParseEnumSet<AbnormalType>(dispelEffects);
        _rate = @params.getInt("rate", 100);
    }

    public override EffectType getEffectType() => EffectType.DISPEL;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null)
            return;

        // The effectlist should already check if it has buff with this abnormal type or not.
        effected.getEffectList().stopEffects(info => !info.getSkill().isIrreplacableBuff() && Rnd.get(100) < _rate &&
            _dispelAbnormals.Contains(info.getSkill().getAbnormalType()), true, true);
    }

    public override int GetHashCode() => HashCode.Combine(_dispelAbnormals.GetSetHashCode(), _rate);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._dispelAbnormals.GetSetComparable(), x._rate));
}