using L2Dn.GameServer.AI;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Servitor move to target player action handler.
 * @author Nik
 */
public class ServitorMove: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (!player.hasServitors())
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_SERVITOR);
			return;
		}

        WorldObject? target = player.getTarget();
		if (target != null)
		{
			foreach (Summon summon in player.getServitors().Values)
			{
				if ((summon != player.getTarget()) && !summon.isMovementDisabled())
				{
					if (summon.isBetrayed())
					{
						player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
						return;
					}

					summon.setFollowStatus(false);
					summon.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, target.Location.Location3D);
				}
			}
		}
	}

	public bool isPetAction()
	{
		return true;
	}
}