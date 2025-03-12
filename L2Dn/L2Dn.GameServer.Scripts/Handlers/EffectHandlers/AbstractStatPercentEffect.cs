using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public abstract class AbstractStatPercentEffect: AbstractEffect
{
    private readonly Stat _stat;
    private readonly double _amount;

    protected AbstractStatPercentEffect(StatSet @params, Stat stat)
    {
        _stat = stat;
        _amount = @params.getDouble("amount", 1);
        if (@params.getEnum("mode", StatModifierType.PER) != StatModifierType.PER)
        {
            LOGGER.Warn(GetType().Name + " can only use PER mode.");
        }
    }

    public double Amount => _amount;

    public override void pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeMul(_stat, _amount / 100 + 1);
    }

    public override int GetHashCode() => HashCode.Combine(_stat, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._stat, x._amount));
}