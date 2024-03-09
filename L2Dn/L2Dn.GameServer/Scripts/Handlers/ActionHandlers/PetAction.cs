using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;

namespace L2Dn.GameServer.Handlers.ActionHandlers;

public class PetAction: IActionHandler
{
	public bool action(Player player, WorldObject target, bool interact)
	{
		// Aggression target lock effect
		if (player.isLockedTarget() && (player.getLockedTarget() != target))
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_ENMITY);
			return false;
		}
		
		bool isOwner = player.getObjectId() == ((Pet)target).getOwner().getObjectId();
		if (isOwner && (player != ((Pet) target).getOwner()))
		{
			((Pet) target).updateRefOwner(player);
		}
		if (player.getTarget() != target)
		{
			// Set the target of the Player player
			player.setTarget(target);
		}
		else if (interact)
		{
			// Check if the pet is attackable (without a forced attack) and isn't dead
			if (target.isAutoAttackable(player) && !isOwner)
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
				player.onActionRequest();
			}
			else if (!((Creature) target).isInsideRadius2D(player, 150))
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
				player.onActionRequest();
			}
			else
			{
				if (isOwner)
				{
					player.sendPacket(new PetStatusShowPacket((Pet) target));
					
					// Notify to scripts
					if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_SUMMON_TALK, target))
					{
						EventDispatcher.getInstance().notifyEventAsync(new OnPlayerSummonTalk((Summon) target), target);
					}
				}
				player.updateNotMoveUntil();
			}
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Pet;
	}
}