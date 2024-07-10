using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Mobius
 */
public class AdminOnline: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_online"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.equalsIgnoreCase("admin_online"))
		{
			List<String> ips = new();
			int total = 0;
			int online = 0;
			int offline = 0;
			int peace = 0;
			int notPeace = 0;
			int instanced = 0;
			int combat = 0;
			foreach (Player player in World.getInstance().getPlayers())
			{
				String? ip = player.getClient()?.IpAddress.ToString();
				if ((ip != null) && !ips.Contains(ip))
				{
					ips.add(ip);
				}
				
				total++;
				
				if (player.isInOfflineMode())
				{
					offline++;
				}
				else if (player.isOnline())
				{
					online++;
				}
				
				if (player.isInsideZone(ZoneId.PEACE))
				{
					peace++;
				}
				else
				{
					notPeace++;
				}
				
				if (player.getInstanceId() > 0)
				{
					instanced++;
				}

				if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player) ||
				    (player.getPvpFlag() != PvpFlagStatus.None) || player.isInsideZone(ZoneId.PVP) ||
				    player.isInsideZone(ZoneId.SIEGE))
				{
					combat++;
				}
			}
			
			BuilderUtil.sendSysMessage(activeChar, "Online Player Report");
			BuilderUtil.sendSysMessage(activeChar, "Total count: " + total);
			BuilderUtil.sendSysMessage(activeChar, "Total online: " + online);
			BuilderUtil.sendSysMessage(activeChar, "Total offline: " + offline);
			BuilderUtil.sendSysMessage(activeChar, "Max connected: " + World.MAX_CONNECTED_COUNT);
			BuilderUtil.sendSysMessage(activeChar, "Unique IPs: " + ips.Count);
			BuilderUtil.sendSysMessage(activeChar, "In peace zone: " + peace);
			BuilderUtil.sendSysMessage(activeChar, "Not in peace zone: " + notPeace);
			BuilderUtil.sendSysMessage(activeChar, "In instances: " + instanced);
			BuilderUtil.sendSysMessage(activeChar, "In combat: " + combat);
		}
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
