using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

public class TeleportBookmark: IPlayerActionHandler
{
	public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
	{
	}

	public bool isPetAction()
	{
		return false;
	}
}