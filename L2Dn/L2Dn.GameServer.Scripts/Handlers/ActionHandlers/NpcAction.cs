using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.ActionHandlers;

public class NpcAction: IActionHandler
{
	/**
	 * Manage actions when a player click on the Npc.<br>
	 * <br>
	 * <b><u>Actions on first click on the Npc (Select it)</u>:</b><br>
	 * <li>Set the Npc as target of the Player player (if necessary)</li>
	 * <li>Send a Server=>Client packet MyTargetSelected to the Player player (display the select window)</li>
	 * <li>If Npc is autoAttackable, send a Server=>Client packet StatusUpdate to the Player in order to update Npc HP bar</li>
	 * <li>Send a Server=>Client packet ValidateLocation to correct the Npc position and heading on the client</li><br>
	 * <br>
	 * <b><u>Actions on second click on the Npc (Attack it/Intercat with it)</u>:</b><br>
	 * <li>Send a Server=>Client packet MyTargetSelected to the Player player (display the select window)</li>
	 * <li>If Npc is autoAttackable, notify the Player AI with AI_INTENTION_ATTACK (after a height verification)</li>
	 * <li>If Npc is NOT autoAttackable, notify the Player AI with AI_INTENTION_INTERACT (after a distance verification) and show message</li><br>
	 * <font color=#FF0000><b><u>Caution</u>: Each group of Server=>Client packet must be terminated by a ActionFailedPacket packet in order to avoid that client wait an other packet</b></font><br>
	 * <br>
	 * <b><u>Example of use</u>:</b><br>
	 * <li>Client packet : Action, AttackRequest</li><br>
	 * @param player The Player that start an action on the Npc
	 */
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (!((Npc) target).canTarget(player))
		{
			return false;
		}

		player.setLastFolkNPC((Npc) target);
		// Check if the Player already target the Npc
		if (target != player.getTarget())
		{
			// Set the target of the Player player
			player.setTarget(target);
			// Check if the player is attackable (without a forced attack)
			if (target.isAutoAttackable(player))
			{
				((Npc) target).getAI(); // wake up ai
			}
		}
		else if (interact)
		{
			// Check if the player is attackable (without a forced attack) and isn't dead
			if (target.isAutoAttackable(player) && !((Creature) target).isAlikeDead())
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
			}
			else if (!target.isAutoAttackable(player))
			{
				// Calculate the distance between the Player and the Npc
				if (!((Npc) target).canInteract(player))
				{
					player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
				}
				else
				{
					Npc npc = (Npc) target;
					if (!player.isSitting()) // Needed for Mystic Tavern Globe
					{
						// Turn NPC to the player.
						player.sendPacket(new MoveToPawnPacket(player, npc, 100));
						if (npc.hasRandomAnimation())
						{
							npc.onRandomAnimation(Rnd.get(8));
						}
					}

					// Stop movement when trying to talk to a moving NPC.
					if (npc.isMoving())
					{
						player.stopMove(null);
					}

					// Open a chat window on client with the text of the Npc
					if (npc.Events.HasSubscribers<OnNpcQuestStart>())
					{
						player.setLastQuestNpcObject(target.ObjectId);
					}

					if (npc.Events.HasSubscribers<OnNpcFirstTalk>())
					{
						npc.Events.NotifyAsync(new OnNpcFirstTalk(npc, player));
					}
					else
					{
						npc.showChatWindow(player);
					}

					if (Config.Character.PLAYER_MOVEMENT_BLOCK_TIME > 0)
					{
						player.updateNotMoveUntil();
					}
					if (npc.isFakePlayer() && GeoEngine.getInstance().canSeeTarget(player, npc))
					{
						player.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, npc);
					}
				}
			}
		}
		return true;
	}

	public InstanceType getInstanceType()
	{
		return InstanceType.Npc;
	}
}