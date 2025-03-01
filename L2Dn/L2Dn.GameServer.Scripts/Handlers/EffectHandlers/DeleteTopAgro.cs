using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DeleteTopAgro: AbstractEffect
{
	private readonly int _chance;

	public DeleteTopAgro(StatSet @params)
	{
		_chance = @params.getInt("chance", 100);
	}

	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return Formulas.calcProbability(_chance, effector, effected, skill);
	}

	public override EffectType getEffectType()
	{
		return EffectType.HATE;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (!effected.isAttackable())
		{
			return;
		}

		Attackable target = (Attackable) effected;
		target.stopHating(target.getMostHated());
		target.setWalking();
		target.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
	}
}