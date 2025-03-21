using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Airship Action player action handler.
 * @author Nik
 */
public class AirshipAction: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        AirShip? airShip = player.getAirShip();
        if (!player.isInAirShip() || airShip == null)
            return;

        switch (data.OptionId)
        {
            case 1: // Steer
            {
                if (airShip.setCaptain(player))
                {
                    player.broadcastUserInfo();
                }

                break;
            }
            case 2: // Cancel Control
            {
                if (airShip.isCaptain(player) && airShip.setCaptain(null))
                {
                    player.broadcastUserInfo();
                }

                break;
            }
            case 3: // Destination Map
            {
                AirShipManager.getInstance().sendAirShipTeleportList(player);
                break;
            }
            case 4: // Exit Airship
            {
                if (airShip.isCaptain(player))
                {
                    if (airShip.setCaptain(null))
                    {
                        player.broadcastUserInfo();
                    }
                }
                else if (airShip.isInDock())
                {
                    airShip.oustPlayer(player);
                }

                break;
            }
        }
    }

    public bool isPetAction()
    {
        return false;
    }
}