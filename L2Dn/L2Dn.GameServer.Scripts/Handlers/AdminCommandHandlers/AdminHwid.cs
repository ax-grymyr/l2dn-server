using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminHwid: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_hwid",
		"admin_hwinfo",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if ((activeChar.getTarget() == null) || !activeChar.getTarget().isPlayer() ||
		    (activeChar.getTarget().getActingPlayer().getClient() == null) ||
		    (activeChar.getTarget().getActingPlayer().getClient()?.HardwareInfo == null))
		{
			return true;
		}

		Player target = activeChar.getTarget().getActingPlayer();

		ClientHardwareInfoHolder hardwareInfo = activeChar.getClient().HardwareInfo;

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/charhwinfo.htm", activeChar);
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		htmlContent.Replace("%name%", target.getName());
		htmlContent.Replace("%macAddress%", hardwareInfo.getMacAddress());
		htmlContent.Replace("%windowsPlatformId%", hardwareInfo.getWindowsPlatformId().ToString());
		htmlContent.Replace("%windowsMajorVersion%", hardwareInfo.getWindowsMajorVersion().ToString());
		htmlContent.Replace("%windowsMinorVersion%", hardwareInfo.getWindowsMinorVersion().ToString());
		htmlContent.Replace("%windowsBuildNumber%", hardwareInfo.getWindowsBuildNumber().ToString());
		htmlContent.Replace("%cpuName%", hardwareInfo.getCpuName());
		htmlContent.Replace("%cpuSpeed%", hardwareInfo.getCpuSpeed().ToString());
		htmlContent.Replace("%cpuCoreCount%", hardwareInfo.getCpuCoreCount().ToString());
		htmlContent.Replace("%vgaName%", hardwareInfo.getVgaName());
		htmlContent.Replace("%vgaDriverVersion%", hardwareInfo.getVgaDriverVersion());
		activeChar.sendPacket(html);
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}