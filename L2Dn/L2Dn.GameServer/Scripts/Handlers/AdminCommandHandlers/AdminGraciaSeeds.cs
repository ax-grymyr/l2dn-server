using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminGraciaSeeds: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_gracia_seeds",
		"admin_kill_tiat",
		"admin_set_sodstate"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		String actualCommand = st.nextToken(); // Get actual command
		String val = "";
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
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/admin/graciaseeds.htm");
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
		helper.Replace("%sodstate%", GraciaSeedsManager.getInstance().getSoDState().ToString());
		helper.Replace("%sodtiatkill%", GraciaSeedsManager.getInstance().getSoDTiatKilled().ToString());
		if (GraciaSeedsManager.getInstance().getSoDTimeForNextStateChange() > TimeSpan.Zero)
		{
			DateTime nextChangeDate = DateTime.UtcNow + (GraciaSeedsManager.getInstance().getSoDTimeForNextStateChange() ?? TimeSpan.Zero);
			helper.Replace("%sodtime%", nextChangeDate.ToString());
		}
		else
		{
			helper.Replace("%sodtime%", "-1");
		}
		activeChar.sendPacket(html);
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
