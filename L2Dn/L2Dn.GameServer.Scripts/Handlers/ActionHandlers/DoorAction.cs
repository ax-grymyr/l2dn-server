using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class DoorAction: IActionHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		// Check if the Player already target the Npc
		if (player.getTarget() != target)
		{
			player.setTarget(target);
		}
		else if (interact)
		{
			Door door = (Door) target;
			ClanHall clanHall = ClanHallData.getInstance().getClanHallByDoorId(door.getId());
			// MyTargetSelected my = new MyTargetSelected(getObjectId(), player.getLevel());
			// player.sendPacket(my);
			if (target.isAutoAttackable(player))
			{
				if (Math.Abs(player.getZ() - target.getZ()) < 400)
				{
					player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
				}
			}
			else if ((player.getClan() != null) && (clanHall != null) && (player.getClanId() == clanHall.getOwnerId()))
			{
				if (!door.IsInsideRadius2D(player, Npc.INTERACTION_DISTANCE))
				{
					player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
				}
				else
				{
					player.addScript(new DoorRequestHolder(door));
					if (!door.isOpen())
					{
						player.sendPacket(new ConfirmDialogPacket(SystemMessageId.WOULD_YOU_LIKE_TO_OPEN_THE_GATE));
					}
					else
					{
						player.sendPacket(new ConfirmDialogPacket(SystemMessageId.WOULD_YOU_LIKE_TO_CLOSE_THE_GATE));
					}
				}
			}
			else if ((player.getClan() != null) && (((Door) target).getFort() != null) && (player.getClan() == ((Door) target).getFort().getOwnerClan()) && ((Door) target).isOpenableBySkill() && !((Door) target).getFort().getSiege().isInProgress())
			{
				if (!target.IsInsideRadius2D(player, Npc.INTERACTION_DISTANCE))
				{
					player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
				}
				else
				{
					player.addScript(new DoorRequestHolder((Door) target));
					if (!((Door) target).isOpen())
					{
						player.sendPacket(new ConfirmDialogPacket(SystemMessageId.WOULD_YOU_LIKE_TO_OPEN_THE_GATE));
					}
					else
					{
						player.sendPacket(new ConfirmDialogPacket(SystemMessageId.WOULD_YOU_LIKE_TO_CLOSE_THE_GATE));
					}
				}
			}
		}
		
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Door;
	}
}