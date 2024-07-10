using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Servitor passive/defend mode player action handler.
 * @author Nik
 */
public class ServitorMode: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (!player.hasServitors())
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_SERVITOR);
			return;
		}

		switch (data.getOptionId())
		{
			case 1: // Passive mode
			{
				player.getServitors().Values.ForEach(s =>
				{
					if (s.isBetrayed())
					{
						player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
						return;
					}

					((SummonAI)s.getAI()).setDefending(false);
				});
				break;
			}
			case 2: // Defending mode
			{
				player.getServitors().Values.ForEach(s =>
				{
					if (s.isBetrayed())
					{
						player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
						return;
					}

					((SummonAI)s.getAI()).setDefending(true);
				});
				break;
			}
		}
	}

	public bool isPetAction()
	{
		return true;
	}
}