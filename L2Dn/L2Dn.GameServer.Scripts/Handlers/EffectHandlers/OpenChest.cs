using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Open Chest effect implementation.
 * @author Adry_85
 */
public class OpenChest: AbstractEffect
{
	public OpenChest(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected is not Chest chest)
			return;

		Player? player = effector.getActingPlayer();
        if (player == null)
            return;

        if (chest.isDead() || player.getInstanceWorld() != chest.getInstanceWorld())
		{
			return;
		}

		if ((player.getLevel() <= 77 && Math.Abs(chest.getLevel() - player.getLevel()) <= 6) || (player.getLevel() >= 78 && Math.Abs(chest.getLevel() - player.getLevel()) <= 5))
		{
			player.broadcastSocialAction(3);
			chest.setSpecialDrop();
			chest.setMustRewardExpSp(false);
			chest.reduceCurrentHp(chest.getMaxHp(), player, skill);
		}
		else
		{
			player.broadcastSocialAction(13);
			chest.addDamageHate(player, 0, 1);
			chest.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, player);
		}
	}
}