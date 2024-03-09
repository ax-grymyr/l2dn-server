using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.PlayerActions;

/**
 * Tactical Signs setting player action handler.
 * @author Nik
 */
public class TacticalSignUse: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if ((!player.isInParty() || (player.getTarget() == null) || !player.getTarget().isCreature()))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}
		
		player.getParty().addTacticalSign(player, data.getOptionId(), (Creature) player.getTarget());
	}

	public bool isPetAction()
	{
		return false;
	}
}