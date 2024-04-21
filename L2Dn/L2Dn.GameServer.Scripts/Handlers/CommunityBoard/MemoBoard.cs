using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Memo board.
 * @author Zoey76
 */
public class MemoBoard: IWriteBoardHandler
{
	private static readonly String[] COMMANDS =
	{
		"_bbsmemo",
		"_bbstopics"
	};
	
	public String[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(String command, Player player)
	{
		CommunityBoardHandler.getInstance().addBypass(player, "Memo Command", command);
		
		String html = HtmCache.getInstance().getHtm("html/CommunityBoard/memo.html", player.getLang());
		CommunityBoardHandler.separateAndSend(html, player);
		return true;
	}
	
	public bool writeCommunityBoardCommand(Player player, String arg1, String arg2, String arg3, String arg4, String arg5)
	{
		// TODO: Implement.
		return false;
	}
}
