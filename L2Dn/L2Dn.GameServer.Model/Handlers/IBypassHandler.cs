using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

/**
 * @author nBd
 */
public interface IBypassHandler
{
	/**
	 * This is the worker method that is called when someone uses an bypass command.
	 * @param command
	 * @param player
	 * @param bypassOrigin
	 * @return success
	 */
	bool useBypass(string command, Player player, Creature bypassOrigin);
	
	/**
	 * This method is called at initialization to register all bypasses automatically.
	 * @return all known bypasses
	 */
	string[] getBypassList();
}