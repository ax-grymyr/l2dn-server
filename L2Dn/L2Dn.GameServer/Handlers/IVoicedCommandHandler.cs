using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

public interface IVoicedCommandHandler
{
	/**
	 * this is the worker method that is called when someone uses an admin command.
	 * @param player
	 * @param command
	 * @param params
	 * @return command success
	 */
	bool useVoicedCommand(String command, Player player, String pars);
	
	/**
	 * this method is called at initialization to register all the item ids automatically
	 * @return all known itemIds
	 */
	String[] getVoicedCommandList();
}