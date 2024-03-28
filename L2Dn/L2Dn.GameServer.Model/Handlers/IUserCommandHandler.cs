using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

public interface IUserCommandHandler
{
	/**
	 * this is the worker method that is called when someone uses an admin command.
	 * @param id
	 * @param player
	 * @return command success
	 */
	bool useUserCommand(int id, Player player);
	
	/**
	 * this method is called at initialization to register all the item ids automatically
	 * @return all known itemIds
	 */
	int[] getUserCommandList();
}