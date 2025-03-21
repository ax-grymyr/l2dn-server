using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Instance Zone Info player action handler.
 * @author St3eT
 */
public class InstanceZoneInfo: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		player.sendPacket(new ExInzoneWaitingPacket(player, false));
	}

	public bool isPetAction()
	{
		return false;
	}
}
