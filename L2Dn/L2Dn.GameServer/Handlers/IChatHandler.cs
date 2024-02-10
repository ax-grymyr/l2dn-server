using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

/**
 * Interface for chat handlers
 * @author durgus
 */
public interface IChatHandler
{
	/**
	 * Handles a specific type of chat messages
	 * @param type
	 * @param player
	 * @param target
	 * @param text
	 * @param shareLocation
	 */
	void handleChat(ChatType type, Player player, String target, String text, bool shareLocation);
	
	/**
	 * Returns a list of all chat types registered to this handler
	 * @return
	 */
	ChatType[] getChatTypeList();
}