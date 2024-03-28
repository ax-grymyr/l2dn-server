using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Grow effect implementation.
 */
public class Grow: AbstractEffect
{
	public Grow(StatSet @params)
	{
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isNpc())
		{
			Npc npc = (Npc) effected;
			npc.setCollisionHeight(npc.getTemplate().getCollisionHeightGrown());
			npc.setCollisionRadius(npc.getTemplate().getCollisionRadiusGrown());
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isNpc())
		{
			Npc npc = (Npc) effected;
			npc.setCollisionHeight(npc.getTemplate().getCollisionHeight());
			npc.setCollisionRadius(npc.getTemplate().getFCollisionRadius());
		}
	}
}