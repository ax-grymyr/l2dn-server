using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author l3x
 */
public class Seed: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!Config.ALLOW_MANOR)
		{
			return false;
		}

        Player? player = playable.getActingPlayer();
        if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		WorldObject? tgt = playable.getTarget();
		if (tgt == null || !tgt.isNpc())
		{
			playable.sendPacket(SystemMessageId.INVALID_TARGET);
			return false;
		}

        if (!tgt.isMonster() || ((Monster) tgt).isRaid() || tgt is Chest)
		{
			playable.sendPacket(SystemMessageId.THE_TARGET_IS_UNAVAILABLE_FOR_SEEDING);
			return false;
		}

		Monster target = (Monster) tgt;
		if (target.isDead())
		{
			playable.sendPacket(SystemMessageId.INVALID_TARGET);
			return false;
		}

        if (target.isSeeded())
        {
            playable.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return false;
        }

        Model.Seed? seed = CastleManorManager.getInstance().getSeed(item.getId());
		if (seed == null)
		{
			return false;
		}

		Castle? taxCastle = target.getTaxCastle();
		if (taxCastle == null || seed.getCastleId() != taxCastle.getResidenceId())
		{
			playable.sendPacket(SystemMessageId.THIS_SEED_MAY_NOT_BE_SOWN_HERE);
			return false;
		}

		target.setSeeded(seed, player);

		List<ItemSkillHolder> skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills != null)
		{
			skills.ForEach(holder => player.useMagic(holder.getSkill(), item, false, false));
		}

		return true;
	}
}