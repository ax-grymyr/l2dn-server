using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Get Agro effect implementation.
 * @author Adry_85, Mobius
 */
public class GetAgro: AbstractEffect
{
	public GetAgro(StatSet @params)
	{
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.AGGRESSION;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected != null) && effected.isAttackable())
		{
			effected.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, effector);
			
			// Monsters from the same clan should assist.
			NpcTemplate template = ((Attackable) effected).getTemplate();
			Set<int> clans = template.getClans();
			if (clans != null)
			{
				World.getInstance().forEachVisibleObjectInRange<Attackable>(effected, template.getClanHelpRange(), nearby =>
				{
					if (!nearby.isMovementDisabled() && nearby.getTemplate().isClan(clans))
					{
						nearby.addDamageHate(effector, 1, 200);
						nearby.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, effector);
					}
				});
			}
		}
	}
}