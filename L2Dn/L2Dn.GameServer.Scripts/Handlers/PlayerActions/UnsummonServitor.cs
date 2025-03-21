using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Unsummon Servitor player action handler.
 * @author St3eT
 */
public class UnsummonServitor: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		bool canUnsummon = true;
		if (player.hasServitors())
		{
			foreach (Summon s in player.getServitors().Values)
			{
				if (s.isBetrayed())
				{
					player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
					canUnsummon = false;
					break;
				}
				else if (s.isAttackingNow() || s.isInCombat() || s.isMovementDisabled())
				{
					player.sendPacket(SystemMessageId.A_SERVITOR_WHOM_IS_ENGAGED_IN_BATTLE_CANNOT_BE_DE_ACTIVATED);
					canUnsummon = false;
					break;
				}
			}

			if (canUnsummon)
			{
				player.getServitors().Values.ForEach(s => s.unSummon(player));
			}
		}
		else
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_SERVITOR);
		}
	}

	public bool isPetAction()
	{
		return true;
	}
}