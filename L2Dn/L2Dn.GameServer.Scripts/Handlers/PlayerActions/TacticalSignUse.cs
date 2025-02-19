using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Tactical Signs setting player action handler.
 * @author Nik
 */
public class TacticalSignUse: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        WorldObject? target = player.getTarget();
        Party? party = player.getParty();
        if (!player.isInParty() || party == null || target == null || !target.isCreature())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return;
        }

        party.addTacticalSign(player, data.getOptionId(), (Creature)target);
    }

    public bool isPetAction()
    {
        return false;
    }
}