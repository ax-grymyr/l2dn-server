using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Transfer Damage effect implementation.
 * @author UnAfraid
 */
public class TransferDamageToPlayer: AbstractStatAddEffect
{
	public TransferDamageToPlayer(StatSet @params): base(@params, Stat.TRANSFER_DAMAGE_TO_PLAYER)
	{
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isPlayable() && effector.isPlayer())
		{
			((Playable) effected).setTransferDamageTo(null);
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayable() && effector.isPlayer())
		{
			((Playable) effected).setTransferDamageTo(effector.getActingPlayer());
		}
	}
}