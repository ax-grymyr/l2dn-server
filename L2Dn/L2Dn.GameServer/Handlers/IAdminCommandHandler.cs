using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

public interface IAdminCommandHandler
{
	/**
	 * this is the worker method that is called when someone uses an admin command.
	 * @param player
	 * @param command
	 * @return command success
	 */
	bool useAdminCommand(String command, Player player);
	
	/**
	 * this method is called at initialization to register all the item ids automatically
	 * @return all known itemIds
	 */
	String[] getAdminCommandList();
}