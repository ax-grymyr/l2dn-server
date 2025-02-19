using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author NosBit
 */
public class MaxHp: AbstractStatEffect
{
	private readonly bool _heal;
	
	public MaxHp(StatSet @params): base(@params, Stat.MAX_HP)
	{
		
		_heal = @params.getBoolean("heal", false);
	}
	
	public override void continuousInstant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_heal)
		{
			ThreadPool.schedule(() =>
			{
				if (!effected.isHpBlocked())
				{
					switch (_mode)
					{
						case StatModifierType.DIFF:
						{
							effected.setCurrentHp(effected.getCurrentHp() + _amount);
							break;
						}
						case StatModifierType.PER:
						{
							effected.setCurrentHp(effected.getCurrentHp() + effected.getMaxHp() * (_amount / 100));
							break;
						}
					}
				}
			}, 100);
		}
	}
}
