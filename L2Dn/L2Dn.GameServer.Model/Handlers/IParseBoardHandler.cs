using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

/**
 * Community Board interface.
 * @author Zoey76
 */
public interface IParseBoardHandler
{
	/**
	 * Parses a community board command.
	 * @param command the command
	 * @param player the player
	 * @return
	 */
	bool parseCommunityBoardCommand(string command, Player player);
	
	/**
	 * Gets the community board commands.
	 * @return the community board commands
	 */
	string[] getCommunityBoardCommands();
}