using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PolearmSingleTarget: AbstractEffect
{
	public PolearmSingleTarget(StatSet @params)
	{
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			effected.getStat().addFixedValue(Stat.PHYSICAL_POLEARM_TARGET_SINGLE, 1.0);
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isPlayer())
		{
			effected.getStat().removeFixedValue(Stat.PHYSICAL_POLEARM_TARGET_SINGLE);
		}
	}
}