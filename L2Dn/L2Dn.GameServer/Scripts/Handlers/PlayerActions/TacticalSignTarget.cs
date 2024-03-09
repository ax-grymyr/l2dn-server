using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Tactical Signs targeting player action handler.
 * @author Nik
 */
public class TacticalSignTarget: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (!player.isInParty())
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}
		
		player.getParty().setTargetBasedOnTacticalSignId(player, data.getOptionId());
	}

	public bool isPetAction()
	{
		return false;
	}
}