using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Target Cancel effect implementation.
 * @author -Nemesiss-, Adry_85
 */
public class TargetCancel: AbstractEffect
{
	private readonly int _chance;
	
	public TargetCancel(StatSet @params)
	{
		_chance = @params.getInt("chance", 100);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !(effected.hasAbnormalType(AbnormalType.ABNORMAL_INVINCIBILITY) || effected.hasAbnormalType(AbnormalType.INVINCIBILITY_SPECIAL) || effected.hasAbnormalType(AbnormalType.INVINCIBILITY)) && Formulas.calcProbability(_chance, effector, effected, skill);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.setTarget(null);
		effected.abortAttack();
		effected.abortCast();
		effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE, effector);
	}
}