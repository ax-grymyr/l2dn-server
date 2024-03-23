using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author malyelfik
 */
public class AdminManor: IAdminCommandHandler
{
	public bool useAdminCommand(String command, Player activeChar)
	{
		CastleManorManager manor = CastleManorManager.getInstance();

		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/admin/manor.htm");
		NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(0, 1, helper);
		helper.Replace("%status%", manor.getCurrentModeName());
		helper.Replace("%change%", manor.getNextModeChange());
		
		StringBuilder sb = new StringBuilder(3400);
		foreach (Castle c in CastleManager.getInstance().getCastles())
		{
			sb.Append("<tr><td>Name:</td><td><font color=008000>" + c.getName() + "</font></td></tr>");
			sb.Append("<tr><td>Current period cost:</td><td><font color=FF9900>" + Util.formatAdena(manor.getManorCost(c.getResidenceId(), false)) + " Adena</font></td></tr>");
			sb.Append("<tr><td>Next period cost:</td><td><font color=FF9900>" + Util.formatAdena(manor.getManorCost(c.getResidenceId(), true)) + " Adena</font></td></tr>");
			sb.Append("<tr><td><font color=808080>--------------------------</font></td><td><font color=808080>--------------------------</font></td></tr>");
		}
		
		helper.Replace("%castleInfo%", sb.ToString());
		activeChar.sendPacket(msg);
		
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return new String[]
		{
			"admin_manor"
		};
	}
}