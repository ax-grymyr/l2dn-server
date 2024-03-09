using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Immobile Pet Buff effect implementation.
 * @author demonia
 */
public class ImmobilePetBuff: AbstractEffect
{
	public ImmobilePetBuff(StatSet @params)
	{
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.setImmobilized(false);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isSummon() && ((effector == effected) || (effector.isPlayer() && (((L2Dn.GameServer.Model.Actor.Summon) effected).getOwner() == effector))))
		{
			effected.setImmobilized(true);
		}
	}
}