using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Skill Turning effect implementation.
/// </summary>
public sealed class SkillTurning: AbstractEffect
{
    private readonly int _chance;
    private readonly bool _staticChance;

    public SkillTurning(StatSet @params)
    {
        _chance = @params.getInt("chance", 100);
        _staticChance = @params.getBoolean("staticChance", false);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return _staticChance ? Formulas.calcProbability(_chance, effector, effected, skill) : Rnd.get(100) < _chance;
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == effector || effected.isRaid())
            return;

        effected.breakCast();
    }

    public override int GetHashCode() => HashCode.Combine(_chance, _staticChance);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._chance, x._staticChance));
}