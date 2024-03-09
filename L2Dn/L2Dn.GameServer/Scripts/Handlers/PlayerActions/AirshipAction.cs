using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers.PlayerActions;

/**
 * Airship Action player action handler.
 * @author Nik
 */
public class AirshipAction: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (!player.isInAirShip())
		{
			return;
		}
		
		switch (data.getOptionId())
		{
			case 1: // Steer
			{
				if (player.getAirShip().setCaptain(player))
				{
					player.broadcastUserInfo();
				}
				break;
			}
			case 2: // Cancel Control
			{
				if (player.getAirShip().isCaptain(player) && player.getAirShip().setCaptain(null))
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
				if (player.getAirShip().isCaptain(player))
				{
					if (player.getAirShip().setCaptain(null))
					{
						player.broadcastUserInfo();
					}
				}
				else if (player.getAirShip().isInDock())
				{
					player.getAirShip().oustPlayer(player);
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