using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Mount/Dismount player action handler.
 * @author Nik
 */
public class Ride: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        player.mountPlayer(player.getPet());
    }

    public bool isPetAction()
    {
        return false;
    }
}