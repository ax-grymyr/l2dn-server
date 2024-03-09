using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Run/Walk player action handler.
 * @author Mobius
 */
public class RunWalk: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
		if (player.isRunning())
		{
			player.setWalking();
		}
		else
		{
			player.setRunning();
		}
	}

	public bool isPetAction()
	{
		return false;
	}
}