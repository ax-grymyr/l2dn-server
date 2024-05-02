using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class PlayerAction: IActionHandler
{
	private const int CURSED_WEAPON_VICTIM_MIN_LEVEL = 21;
	
	/**
	 * Manage actions when a player click on this Player.<br>
	 * <br>
	 * <b><u>Actions on first click on the Player (Select it)</u>:</b><br>
	 * <li>Set the target of the player</li>
	 * <li>Send a Server=>Client packet MyTargetSelected to the player (display the select window)</li><br>
	 * <br>
	 * <b><u>Actions on second click on the Player (Follow it/Attack it/Intercat with it)</u>:</b><br>
	 * <li>Send a Server=>Client packet MyTargetSelected to the player (display the select window)</li>
	 * <li>If target Player has a Private Store, notify the player AI with AI_INTENTION_INTERACT</li>
	 * <li>If target Player is autoAttackable, notify the player AI with AI_INTENTION_ATTACK</li>
	 * <li>If target Player is NOT autoAttackable, notify the player AI with AI_INTENTION_FOLLOW</li><br>
	 * <br>
	 * <b><u>Example of use</u>:</b><br>
	 * <li>Client packet : Action, AttackRequest</li><br>
	 * @param player The player that start an action on target Player
	 */
	public bool action(Player player, WorldObject target, bool interact)
	{
		// Check if the Player is confused
		if (player.isControlBlocked())
		{
			return false;
		}
		
		// Aggression target lock effect
		if (player.isLockedTarget() && (player.getLockedTarget() != target))
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CHANGE_ENMITY);
			return false;
		}
		
		// Check if the player already target this Player
		if (player.getTarget() != target)
		{
			// Set the target of the player
			player.setTarget(target);
		}
		else if (interact)
		{
			// Check if this Player has a Private Store
			Player targetPlayer = target.getActingPlayer();
			if (targetPlayer.getPrivateStoreType() != PrivateStoreType.NONE)
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
			}
			else
			{
				// Check if this Player is autoAttackable
				if (target.isAutoAttackable(player))
				{
					// Player with level < 21 can't attack a cursed weapon holder
					// And a cursed weapon holder can't attack players with level < 21
					if ((targetPlayer.isCursedWeaponEquipped() && (player.getLevel() < CURSED_WEAPON_VICTIM_MIN_LEVEL)) //
						|| (player.isCursedWeaponEquipped() && (targetPlayer.getLevel() < CURSED_WEAPON_VICTIM_MIN_LEVEL)))
					{
						player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					}
					else
					{
						player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
						player.onActionRequest();
					}
				}
				else
				{
					// This Action Failed packet avoids player getting stuck when clicking three or more times
					player.sendPacket(ActionFailedPacket.STATIC_PACKET);
					if (GeoEngine.getInstance().canMoveToTarget(player.Location.Location3D, target.Location.Location3D))
					{
						player.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, target);
					}
				}
			}
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Player;
	}
}