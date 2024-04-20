using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AbstractStatPercentEffect: AbstractEffect
{
	private readonly Stat _stat;
	protected double _amount;
	
	public AbstractStatPercentEffect(StatSet @params, Stat stat)
	{
		_stat = stat;
		_amount = @params.getDouble("amount", 1);
		if (@params.getEnum("mode", StatModifierType.PER) != StatModifierType.PER)
		{
			LOGGER.Warn(GetType().Name + " can only use PER mode.");
		}
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeMul(_stat, (_amount / 100) + 1);
	}
}