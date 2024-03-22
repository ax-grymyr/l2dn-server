using System.Diagnostics;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
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

			HtmlPacketHelper helper =
				new HtmlPacketHelper(HtmCache.getInstance().getHtm(activeChar, "html/admin/serverinfo.htm"));

			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(helper);
			helper.Replace("%os_name%", Environment.OSVersion.Platform.ToString());
			helper.Replace("%os_ver%", Environment.OSVersion.VersionString);
			helper.Replace("%slots%", getPlayersCount("ALL") + "/" + Config.MAXIMUM_ONLINE_USERS);
			helper.Replace("%gameTime%",
				GameTimeTaskManager.getInstance().getGameHour() + ":" +
				GameTimeTaskManager.getInstance().getGameMinute());
			
			helper.Replace("%dayNight%", GameTimeTaskManager.getInstance().isNight() ? "Night" : "Day");
			helper.Replace("%geodata%", Config.PATHFINDING > 0 ? "Enabled" : "Disabled");
			helper.Replace("%serverTime%", DateTime.UtcNow.ToString("u"));
			helper.Replace("%serverUpTime%", getServerUpTime());
			helper.Replace("%onlineAll%", getPlayersCount("ALL").ToString());
			helper.Replace("%offlineTrade%", getPlayersCount("OFF_TRADE").ToString());
			helper.Replace("%onlineGM%", getPlayersCount("GM").ToString());
			helper.Replace("%onlineReal%", getPlayersCount("ALL_REAL").ToString());
			helper.Replace("%usedMem%", (process.PrivateMemorySize64 / 0x100000) + " Mb");
			helper.Replace("%freeMem%", (0 / 0x100000) + " Mb"); // TODO
			helper.Replace("%totalMem%", (0 / 0x100000) + " Mb");
			helper.Replace("%live%", process.Threads.Count.ToString());
			helper.Replace("%nondaemon%", "-");
			helper.Replace("%daemon%", "-");
			helper.Replace("%peak%", "-");
			helper.Replace("%totalstarted%", "-");

			helper.Replace("%gcol%", "-");
			helper.Replace("%colcount%", "-");
			helper.Replace("%coltime%", "-");

			activeChar.sendPacket(html);
		}

		return true;
	}
	
	private String getServerUpTime()
	{
		TimeSpan time = DateTime.UtcNow - GameServer.ServerStarted;
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
					if ((onlinePlayer != null) && (onlinePlayer.getClient() != null) && !onlinePlayer.getClient().IsDetached)
					{
						realPlayers.add(onlinePlayer.getIPAddress());
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
