using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.PlayerActions;

/**
 * Servitor hold position player action handler.
 * @author Nik
 */
public class ServitorHold: IPlayerActionHandler
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
			
			((SummonAI) s.getAI()).notifyFollowStatusChange();
		});
	}
	
	public bool isPetAction()
	{
		return true;
	}
}