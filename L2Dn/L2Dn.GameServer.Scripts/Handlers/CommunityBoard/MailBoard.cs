using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.CommunityBoard;

/**
 * Mail board.
 * @author Zoey76
 */
public class MailBoard: IWriteBoardHandler
{
	private static readonly string[] COMMANDS =
	{
		"_maillist"
	};
	
	public string[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(string command, Player player)
	{
		CommunityBoardHandler.getInstance().addBypass(player, "Mail Command", command);
		
		string html = HtmCache.getInstance().getHtm("html/CommunityBoard/mail.html", player.getLang());
		CommunityBoardHandler.separateAndSend(html, player);
		return true;
	}
	
	public bool writeCommunityBoardCommand(Player player, string arg1, string arg2, string arg3, string arg4, string arg5)
	{
		// TODO: Implement.
		return false;
	}
}
