using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Servitor stop action player action handler.
 * @author Nik
 */
public class ServitorStop: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (!player.hasServitors())
		{
			player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_SERVITOR);
			return;
		}
		
		player.getServitors().values().forEach(s =>
		{
			if (s.isBetrayed())
			{
				player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
				return;
			}
			
			s.cancelAction();
		});
	}
	
	public bool isPetAction()
	{
		return true;
	}
}