using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Sow effect implementation.
 * @author Adry_85, l3x
 */
public class Sow: AbstractEffect
{
	public Sow(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
        Player? player = effector.getActingPlayer();
		if (!effector.isPlayer() || !effected.isMonster() || player == null)
        {
            return;
        }

		Monster target = (Monster) effected;
		if (target.isDead() || !target.getTemplate().canBeSown() || !target.isSeeded() || target.getSeederId() != player.ObjectId)
		{
			return;
		}

		// Consuming used seed
        Seed? seed = target.getSeed();
		if (seed == null || !player.destroyItemByItemId("Consume", seed.getSeedId(), 1, target, false))
		{
			return;
		}

		SystemMessagePacket sm;
		if (CalcSuccess(player, target, seed))
        {
            string? sound = QuestSound.ITEMSOUND_QUEST_ITEMGET.GetSoundName();
            if (sound != null)
			    player.sendPacket(new PlaySoundPacket(sound));

			target.setSeeded(player.getActingPlayer());
			sm = new SystemMessagePacket(SystemMessageId.THE_SEED_WAS_SUCCESSFULLY_SOWN);
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.THE_SEED_WAS_NOT_SOWN);
		}

		Party? party = player.getParty();
		if (party != null)
		{
			party.broadcastPacket(sm);
		}
		else
		{
			player.sendPacket(sm);
		}

		// TODO: Mob should not aggro on player, this way doesn't work really nice
		target.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}

	private static bool CalcSuccess(Creature creature, Creature target, Seed seed)
	{
		// TODO: check all the chances
		int minlevelSeed = seed.getLevel() - 5;
		int maxlevelSeed = seed.getLevel() + 5;
		int levelPlayer = creature.getLevel(); // Attacker Level
		int levelTarget = target.getLevel(); // target Level
		int basicSuccess = seed.isAlternative() ? 20 : 90;

		// seed level
		if (levelTarget < minlevelSeed)
		{
			basicSuccess -= 5 * (minlevelSeed - levelTarget);
		}
		if (levelTarget > maxlevelSeed)
		{
			basicSuccess -= 5 * (levelTarget - maxlevelSeed);
		}

		// 5% decrease in chance if player level
		// is more than +/- 5 levels to _target's_ level
		int diff = levelPlayer - levelTarget;
		if (diff < 0)
		{
			diff = -diff;
		}
		if (diff > 5)
		{
			basicSuccess -= 5 * (diff - 5);
		}

		// chance can't be less than 1%
        basicSuccess = Math.Max(basicSuccess, 1);
		return Rnd.get(99) < basicSuccess;
	}
}