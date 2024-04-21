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
	private static readonly String[] COMMANDS =
	{
		"_maillist"
	};
	
	public String[] getCommunityBoardCommands()
	{
		return COMMANDS;
	}
	
	public bool parseCommunityBoardCommand(String command, Player player)
	{
		CommunityBoardHandler.getInstance().addBypass(player, "Mail Command", command);
		
		String html = HtmCache.getInstance().getHtm("html/CommunityBoard/mail.html", player.getLang());
		CommunityBoardHandler.separateAndSend(html, player);
		return true;
	}
	
	public bool writeCommunityBoardCommand(Player player, String arg1, String arg2, String arg3, String arg4, String arg5)
	{
		// TODO: Implement.
		return false;
	}
}
