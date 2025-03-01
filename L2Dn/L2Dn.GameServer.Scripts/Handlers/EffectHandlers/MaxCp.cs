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
 * @author Nik
 */
public class MaxCp: AbstractStatEffect
{
	private readonly bool _heal;

	public MaxCp(StatSet @params): base(@params, Stat.MAX_CP)
	{

		_heal = @params.getBoolean("heal", false);
	}

	public override void continuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (_heal)
		{
			ThreadPool.schedule(() =>
			{
				switch (_mode)
				{
					case StatModifierType.DIFF:
					{
						effected.setCurrentCp(effected.getCurrentCp() + _amount);
						break;
					}
					case StatModifierType.PER:
					{
						effected.setCurrentCp(effected.getCurrentCp() + effected.getMaxCp() * (_amount / 100));
						break;
					}
				}
			}, 100);
		}
	}
}