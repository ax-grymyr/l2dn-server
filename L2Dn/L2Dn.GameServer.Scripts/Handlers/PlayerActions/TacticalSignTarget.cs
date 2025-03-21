using L2Dn.GameServer.Dto;
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
        Party? party = player.getParty();
        if (!player.isInParty() || party == null)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return;
        }

        party.setTargetBasedOnTacticalSignId(player, data.OptionId);
    }

    public bool isPetAction()
    {
        return false;
    }
}