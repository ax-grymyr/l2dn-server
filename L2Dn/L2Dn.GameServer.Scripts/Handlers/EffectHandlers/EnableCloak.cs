using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Enable Cloak effect implementation.
 * @author Adry_85
 */
public class EnableCloak: AbstractEffect
{
	public EnableCloak(StatSet @params)
	{
	}

	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effector != null && effected != null && effected.isPlayer();
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.getActingPlayer()?.getStat().setCloakSlotStatus(true);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getActingPlayer()?.getStat().setCloakSlotStatus(false);
	}
}