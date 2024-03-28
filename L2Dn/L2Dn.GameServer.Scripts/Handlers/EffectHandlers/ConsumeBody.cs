using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Consume Body effect implementation.
 * @author Mobius
 */
public class ConsumeBody: AbstractEffect
{
	public ConsumeBody(StatSet @params)
	{
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isDead() //
			|| (effector.getTarget() != effected) //
			|| (!effected.isNpc() && !effected.isSummon()) //
			|| (effected.isSummon() && (effector != effected.getActingPlayer())))
		{
			return;
		}
		
		if (effected.isNpc())
		{
			((Npc) effected).endDecayTask();
		}
	}
}