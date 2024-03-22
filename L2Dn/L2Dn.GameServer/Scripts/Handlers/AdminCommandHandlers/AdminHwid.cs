using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminHwid: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_hwid",
		"admin_hwinfo"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if ((activeChar.getTarget() == null) || !activeChar.getTarget().isPlayer() ||
		    (activeChar.getTarget().getActingPlayer().getClient() == null) ||
		    (activeChar.getTarget().getActingPlayer().getClient()?.HardwareInfo == null))
		{
			return true;
		}

		Player target = activeChar.getTarget().getActingPlayer();

		ClientHardwareInfoHolder hardwareInfo = activeChar.getClient().HardwareInfo;

		HtmlPacketHelper helper =
			new HtmlPacketHelper(HtmCache.getInstance().getHtm(activeChar, "html/admin/charhwinfo.htm"));
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
		helper.Replace("%name%", target.getName());
		helper.Replace("%macAddress%", hardwareInfo.getMacAddress());
		helper.Replace("%windowsPlatformId%", hardwareInfo.getWindowsPlatformId().ToString());
		helper.Replace("%windowsMajorVersion%", hardwareInfo.getWindowsMajorVersion().ToString());
		helper.Replace("%windowsMinorVersion%", hardwareInfo.getWindowsMinorVersion().ToString());
		helper.Replace("%windowsBuildNumber%", hardwareInfo.getWindowsBuildNumber().ToString());
		helper.Replace("%cpuName%", hardwareInfo.getCpuName());
		helper.Replace("%cpuSpeed%", hardwareInfo.getCpuSpeed().ToString());
		helper.Replace("%cpuCoreCount%", hardwareInfo.getCpuCoreCount().ToString());
		helper.Replace("%vgaName%", hardwareInfo.getVgaName());
		helper.Replace("%vgaDriverVersion%", hardwareInfo.getVgaDriverVersion());
		activeChar.sendPacket(html);
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}