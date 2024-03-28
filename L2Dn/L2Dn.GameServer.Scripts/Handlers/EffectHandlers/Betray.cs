using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Betray effect implementation.
 * @author decad
 */
public class Betray: AbstractEffect
{
	public Betray(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effector.isPlayer() && effected.isSummon();
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.BETRAYED.getMask();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, effected.getActingPlayer());
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}
}