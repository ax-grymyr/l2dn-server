using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Targeting disable effect implementation. When affected, player will lose target and be unable to target for the duration.
 * @author Nik
 */
public class DisableTargeting: AbstractEffect
{
	public DisableTargeting(StatSet @params)
	{
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.setTarget(null);
		effected.abortAttack();
		effected.abortCast();
		effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.TARGETING_DISABLED.getMask();
	}
}