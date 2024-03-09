using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers.ActionHandlers;

public class ArtefactAction: IActionHandler
{
	/**
	 * Manage actions when a player click on the Artefact.<br>
	 * <br>
	 * <b><u>Actions</u>:</b><br>
	 * <li>Set the Npc as target of the Player player (if necessary)</li>
	 * <li>Send a Server=>Client packet MyTargetSelected to the Player player (display the select window)</li>
	 * <li>Send a Server=>Client packet ValidateLocation to correct the Npc position and heading on the client</li><br>
	 * <br>
	 * <b><u>Example of use</u>:</b><br>
	 * <li>Client packet : Action, AttackRequest</li>
	 */
	public bool action(Player player, WorldObject target, bool interact)
	{
		if (!((Npc) target).canTarget(player))
		{
			return false;
		}
		if (player.getTarget() != target)
		{
			player.setTarget(target);
		}
		// Calculate the distance between the Player and the Npc
		else if (interact && !((Npc) target).canInteract(player))
		{
			// Notify the Player AI with AI_INTENTION_INTERACT
			player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, target);
		}
		return true;
	}
	
	public InstanceType getInstanceType()
	{
		return InstanceType.Artefact;
	}
}