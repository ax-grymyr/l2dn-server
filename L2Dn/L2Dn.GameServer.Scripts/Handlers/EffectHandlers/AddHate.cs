using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Add Hate effect implementation.
/// </summary>
public sealed class AddHate: AbstractEffect
{
    private readonly double _power;
    private readonly bool _affectSummoner;

    public AddHate(StatSet @params)
    {
        _power = @params.getDouble("power", 0);
        _affectSummoner = @params.getBoolean("affectSummoner", false);
    }

    public override bool isInstant() => true;

    public override void instant(Creature creature, Creature effected, Skill skill, Item? item)
    {
        Creature effector = creature;
        Creature? summoner = effector.getSummoner();
        if (_affectSummoner && summoner != null)
            effector = summoner;

        if (!effected.isAttackable())
            return;

        double val = _power;
        if (val > 0)
        {
            ((Attackable)effected).addDamageHate(effector, 0, (int)val);
            effected.setRunning();
        }
        else if (val < 0)
        {
            ((Attackable)effected).reduceHate(effector, (int)-val);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_power, _affectSummoner);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x._affectSummoner));
}