using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Friends board.
 * @author Zoey76
 */
public class FriendsBoard: IParseBoardHandler
{
	private static readonly String[] COMMANDS =
	{
		"_friendlist",
		"_friendblocklist"
	};
	
	public String[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(String command, Player player)
	{
		if (command.equals("_friendlist"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Friends List", command);
			
			String html = HtmCache.getInstance().getHtm("html/CommunityBoard/friends_list.html", player.getLang());
			CommunityBoardHandler.separateAndSend(html, player);
		}
		else if (command.equals("_friendblocklist"))
		{
			CommunityBoardHandler.getInstance().addBypass(player, "Ignore list", command);
			
			String html = HtmCache.getInstance().getHtm("html/CommunityBoard/friends_block_list.html", player.getLang());
			CommunityBoardHandler.separateAndSend(html, player);
		}
		return true;
	}
}
