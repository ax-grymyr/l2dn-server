using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class AbstractStatAddEffect: AbstractEffect
{
	private readonly Stat _stat;
	protected double _amount;
	
	public AbstractStatAddEffect(StatSet @params, Stat stat)
	{
		_stat = stat;
		_amount = @params.getDouble("amount", 0);
		if (@params.getEnum("mode", StatModifierType.DIFF) != StatModifierType.DIFF)
		{
			LOGGER.Warn(GetType().Name + " can only use DIFF mode.");
		}
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		effected.getStat().mergeAdd(_stat, _amount);
	}
}