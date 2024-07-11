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
	private static readonly string[] COMMANDS =
	{
		"_bbsmemo",
		"_bbstopics"
	};
	
	public string[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(string command, Player player)
	{
		CommunityBoardHandler.getInstance().addBypass(player, "Memo Command", command);
		
		string html = HtmCache.getInstance().getHtm("html/CommunityBoard/memo.html", player.getLang());
		CommunityBoardHandler.separateAndSend(html, player);
		return true;
	}
	
	public bool writeCommunityBoardCommand(Player player, string arg1, string arg2, string arg3, string arg4, string arg5)
	{
		// TODO: Implement.
		return false;
	}
}
