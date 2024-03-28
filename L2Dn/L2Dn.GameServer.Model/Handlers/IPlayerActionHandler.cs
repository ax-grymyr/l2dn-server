using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public interface IPlayerActionHandler
{
	void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed);

	bool isPetAction();
}