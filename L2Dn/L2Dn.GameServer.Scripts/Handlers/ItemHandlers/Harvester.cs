using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author l3x
 */
public class Harvester: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Harvester));

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

		List<ItemSkillHolder>? skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills == null)
		{
			_logger.Warn(GetType().Name + ": is missing skills!");
			return false;
		}

		WorldObject? target = player.getTarget();
		if (target == null || !target.isMonster() || !((Creature)target).isDead())
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}

		skills.ForEach(holder => player.useMagic(holder.getSkill(), item, false, false));
		return true;
	}
}