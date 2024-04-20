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
 * @author Sdw
 */
public class MaxMp: AbstractStatEffect
{
	private readonly bool _heal;
	
	public MaxMp(StatSet @params): base(@params, Stat.MAX_MP)
	{
		
		_heal = @params.getBoolean("heal", false);
	}
	
	public override void continuousInstant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_heal)
		{
			ThreadPool.schedule(() =>
			{
				switch (_mode)
				{
					case StatModifierType.DIFF:
					{
						effected.setCurrentMp(effected.getCurrentMp() + _amount);
						break;
					}
					case StatModifierType.PER:
					{
						effected.setCurrentMp(effected.getCurrentMp() + (effected.getMaxMp() * (_amount / 100)));
						break;
					}
				}
			}, 100);
		}
	}
}
