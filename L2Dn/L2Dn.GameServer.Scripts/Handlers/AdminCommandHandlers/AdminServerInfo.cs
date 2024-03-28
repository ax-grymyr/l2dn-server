using System.Diagnostics;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author St3eT
 */
public class AdminServerInfo: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_serverinfo"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		if (command.equals("admin_serverinfo"))
		{
			Process process = Process.GetCurrentProcess();

			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/serverinfo.htm", activeChar);
			htmlContent.Replace("%os_name%", Environment.OSVersion.Platform.ToString());
			htmlContent.Replace("%os_ver%", Environment.OSVersion.VersionString);
			htmlContent.Replace("%slots%", getPlayersCount("ALL") + "/" + Config.MAXIMUM_ONLINE_USERS);
			htmlContent.Replace("%gameTime%",
				GameTimeTaskManager.getInstance().getGameHour() + ":" +
				GameTimeTaskManager.getInstance().getGameMinute());
			
			htmlContent.Replace("%dayNight%", GameTimeTaskManager.getInstance().isNight() ? "Night" : "Day");
			htmlContent.Replace("%geodata%", Config.PATHFINDING > 0 ? "Enabled" : "Disabled");
			htmlContent.Replace("%serverTime%", DateTime.UtcNow.ToString("u"));
			htmlContent.Replace("%serverUpTime%", getServerUpTime());
			htmlContent.Replace("%onlineAll%", getPlayersCount("ALL").ToString());
			htmlContent.Replace("%offlineTrade%", getPlayersCount("OFF_TRADE").ToString());
			htmlContent.Replace("%onlineGM%", getPlayersCount("GM").ToString());
			htmlContent.Replace("%onlineReal%", getPlayersCount("ALL_REAL").ToString());
			htmlContent.Replace("%usedMem%", (process.PrivateMemorySize64 / 0x100000) + " Mb");
			htmlContent.Replace("%freeMem%", (0 / 0x100000) + " Mb"); // TODO
			htmlContent.Replace("%totalMem%", (0 / 0x100000) + " Mb");
			htmlContent.Replace("%live%", process.Threads.Count.ToString());
			htmlContent.Replace("%nondaemon%", "-");
			htmlContent.Replace("%daemon%", "-");
			htmlContent.Replace("%peak%", "-");
			htmlContent.Replace("%totalstarted%", "-");

			htmlContent.Replace("%gcol%", "-");
			htmlContent.Replace("%colcount%", "-");
			htmlContent.Replace("%coltime%", "-");

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 0, htmlContent);
			activeChar.sendPacket(html);
		}

		return true;
	}
	
	private String getServerUpTime()
	{
		TimeSpan time = DateTime.UtcNow - ServerInfo.ServerStarted;
		return (int)time.TotalDays + " Days, " + time.Hours + " Hours, " + time.Minutes + " Minutes";
	}
	
	private int getPlayersCount(String type)
	{
		switch (type)
		{
			case "ALL":
			{
				return World.getInstance().getPlayers().Count;
			}
			case "OFF_TRADE":
			{
				int offlineCount = 0;
				
				ICollection<Player> objs = World.getInstance().getPlayers();
				foreach (Player player in objs)
				{
					if ((player.getClient() == null) || player.getClient().IsDetached)
					{
						offlineCount++;
					}
				}
				return offlineCount;
			}
			case "GM":
			{
				int onlineGMcount = 0;
				foreach (Player gm in AdminData.getInstance().getAllGms(true))
				{
					if ((gm != null) && gm.isOnline() && (gm.getClient() != null) && !gm.getClient().IsDetached)
					{
						onlineGMcount++;
					}
				}
				return onlineGMcount;
			}
			case "ALL_REAL":
			{
				Set<String> realPlayers = new();
				foreach (Player onlinePlayer in World.getInstance().getPlayers())
				{
					GameSession? client = onlinePlayer?.getClient(); 
					if ((client != null) && !client.IsDetached)
					{
						realPlayers.add(client.IpAddress.ToString());
					}
				}
				
				return realPlayers.size();
			}
		}
		return 0;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
