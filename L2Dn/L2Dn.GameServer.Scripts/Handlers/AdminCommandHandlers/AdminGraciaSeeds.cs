using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminGraciaSeeds: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_gracia_seeds",
		"admin_kill_tiat",
		"admin_set_sodstate",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken(); // Get actual command
		string val = "";
		if (st.countTokens() >= 1)
		{
			val = st.nextToken();
		}
		
		if (actualCommand.equalsIgnoreCase("admin_kill_tiat"))
		{
			GraciaSeedsManager.getInstance().increaseSoDTiatKilled();
		}
		else if (actualCommand.equalsIgnoreCase("admin_set_sodstate"))
		{
			GraciaSeedsManager.getInstance().setSoDState(int.Parse(val), true);
		}
		
		showMenu(activeChar);
		return true;
	}
	
	private void showMenu(Player activeChar)
	{
		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/graciaseeds.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%sodstate%", GraciaSeedsManager.getInstance().getSoDState().ToString());
		htmlContent.Replace("%sodtiatkill%", GraciaSeedsManager.getInstance().getSoDTiatKilled().ToString());
		if (GraciaSeedsManager.getInstance().getSoDTimeForNextStateChange() > TimeSpan.Zero)
		{
			DateTime nextChangeDate = DateTime.UtcNow + (GraciaSeedsManager.getInstance().getSoDTimeForNextStateChange() ?? TimeSpan.Zero);
			htmlContent.Replace("%sodtime%", nextChangeDate.ToString());
		}
		else
		{
			htmlContent.Replace("%sodtime%", "-1");
		}
		activeChar.sendPacket(html);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
