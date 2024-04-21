using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Homepage board.
 * @author Zoey76
 */
public class HomepageBoard: IParseBoardHandler
{
	private static readonly String[] COMMANDS =
	{
		"_bbslink"
	};
	
	public String[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(String command, Player player)
    {
        CommunityBoardHandler.separateAndSend(
            HtmCache.getInstance().getHtm("html/CommunityBoard/homepage.html", player.getLang()), player);
        
		return true;
	}
}
