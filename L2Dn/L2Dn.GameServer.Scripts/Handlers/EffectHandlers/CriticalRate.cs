using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw, Mobius
 */
public class CriticalRate: AbstractConditionalHpEffect
{
	public CriticalRate(StatSet @params): base(@params, Stat.CRITICAL_RATE)
	{
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		if (!_conditions.isEmpty())
		{
			foreach (Condition cond in _conditions)
			{
				if (!cond.test(effected, effected, skill))
				{
					return;
				}
			}
		}
		
		switch (_mode)
		{
			case StatModifierType.DIFF:
			{
				effected.getStat().mergeAdd(_addStat, _amount);
				break;
			}
			case StatModifierType.PER:
			{
				effected.getStat().mergeMul(_mulStat, (_amount / 100));
				break;
			}
		}
	}
}