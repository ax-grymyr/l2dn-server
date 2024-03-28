using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Target Me effect implementation.
 * @author -Nemesiss-
 */
public class TargetMe: AbstractEffect
{
	public TargetMe(StatSet @params)
	{
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isPlayable())
		{
			((Playable) effected).setLockedTarget(null);
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayable())
		{
			if (effected.getTarget() != effector)
			{
				effected.setTarget(effector);
			}
			
			((Playable) effected).setLockedTarget(effector);
		}
	}
}