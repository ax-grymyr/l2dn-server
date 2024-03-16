using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events.Impl.Summons;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class SummonAction: IActionHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		// Aggression target lock effect
		if (player.isLockedTarget() && (player.getLockedTarget() != target))
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_ENMITY);
			return false;
		}

		Summon summon = (Summon)target;
		if ((player == summon.getOwner()) && (player.getTarget() == target))
		{
			player.sendPacket(new PetStatusShowPacket((Summon) target));
			player.updateNotMoveUntil();
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			
			// Notify to scripts
			if (summon.Events.HasSubscribers<OnSummonTalk>())
			{
				summon.Events.NotifyAsync(new OnSummonTalk(summon));
			}
		}
		else if (player.getTarget() != target)
		{
			player.setTarget(target);
		}
		else if (interact)
		{
			if (target.isAutoAttackable(player))
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
				player.onActionRequest();
			}
			else
			{
				// This Action Failed packet avoids player getting stuck when clicking three or more times
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				if (((Summon) target).isInsideRadius2D(player, 150))
				{
					player.updateNotMoveUntil();
				}
				else if (GeoEngine.getInstance().canMoveToTarget(player, target))
				{
					player.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, target);
				}
			}
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Summon;
	}
}