using System.Text;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Clan Penalty user command.
 * @author Tempy
 */
public class ClanPenalty: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		100
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		bool penalty = false;
		StringBuilder htmlContent = new StringBuilder();
		htmlContent.Append("<html><body><center><table width=270 border=0 bgcolor=111111><tr><td width=170>Penalty</td><td width=100 align=center>Expiration Date</td></tr></table><table width=270 border=0><tr>");
		
		if (player.getClanJoinExpiryTime() > DateTime.UtcNow)
		{
			htmlContent.Append("<td width=170>Unable to join a clan.</td><td width=100 align=center>");
			htmlContent.Append(player.getClanJoinExpiryTime()?.ToString("yyyy-MM-dd"));
			htmlContent.Append("</td>");
			penalty = true;
		}
		
		if (player.getClanCreateExpiryTime() > DateTime.UtcNow)
		{
			htmlContent.Append("<td width=170>Unable to create a clan.</td><td width=100 align=center>");
			htmlContent.Append(player.getClanCreateExpiryTime()?.ToString("yyyy-MM-dd"));
			htmlContent.Append("</td>");
			penalty = true;
		}
		
		if ((player.getClan() != null) && player.getClan().getCharPenaltyExpiryTime() > DateTime.UtcNow)
		{
			htmlContent.Append("<td width=170>Unable to invite a clan member.</td><td width=100 align=center>");
			htmlContent.Append(player.getClan().getCharPenaltyExpiryTime()?.ToString("yyyy-MM-dd"));
			htmlContent.Append("</td>");
			penalty = true;
		}
		
		if (!penalty)
		{
			htmlContent.Append("<td width=170>No penalty is imposed.</td><td width=100 align=center></td>");
		}
		
		htmlContent.Append("</tr></table><img src=\"L2UI.SquareWhite\" width=270 height=1></center></body></html>");
		
		NpcHtmlMessagePacket penaltyHtml = new NpcHtmlMessagePacket(htmlContent.ToString());
		player.sendPacket(penaltyHtml);
		
		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}