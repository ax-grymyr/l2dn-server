using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Events.Impl.Summons;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

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

		Pet pet = (Pet)target;
		bool isOwner = player.getObjectId() == pet.getOwner().getObjectId();
		if (isOwner && (player != pet.getOwner()))
		{
			pet.updateRefOwner(player);
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
			else if (!pet.IsInsideRadius2D(player, 150))
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
				player.onActionRequest();
			}
			else
			{
				if (isOwner)
				{
					player.sendPacket(new PetStatusShowPacket(pet));
					
					// Notify to scripts
					if (pet.Events.HasSubscribers<OnSummonTalk>())
					{
						pet.Events.NotifyAsync(new OnSummonTalk(pet));
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