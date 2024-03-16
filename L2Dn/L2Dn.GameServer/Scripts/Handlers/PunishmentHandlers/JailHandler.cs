using L2Dn.GameServer.Data;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.PunishmentHandlers;

/**
 * This class handles jail punishment.
 * @author UnAfraid
 */
public class JailHandler: IPunishmentHandler
{
	public JailHandler()
	{
		// Register global listener
		GlobalEvents.Global.Subscribe<OnPlayerLogin>(this, onPlayerLogin);
	}
	
	private void onPlayerLogin(OnPlayerLogin @event)
	{
		Player player = @event.getPlayer();
		if (player.isJailed() && !player.isInsideZone(ZoneId.JAIL))
		{
			applyToPlayer(null, player);
		}
		else if (!player.isJailed() && player.isInsideZone(ZoneId.JAIL) && !player.isGM())
		{
			removeFromPlayer(player);
		}
	}
	
	public void onStart(PunishmentTask task)
	{
		switch (task.getAffect())
		{
			case PunishmentAffect.CHARACTER:
			{
				int objectId = int.Parse(task.getKey());
				Player player = World.getInstance().getPlayer(objectId);
				if (player != null)
				{
					applyToPlayer(task, player);
				}
				break;
			}
			case PunishmentAffect.ACCOUNT:
			{
				String account = task.getKey();
				// GameClient client = LoginServerThread.getInstance().getClient(account);
				// if (client != null)
				// {
				// 	Player player = client.getPlayer();
				// 	if (player != null)
				// 	{
				// 		applyToPlayer(task, player);
				// 	}
				// }
				break;
			}
			case PunishmentAffect.IP:
			{
				String ip = task.getKey();
				foreach (Player player in World.getInstance().getPlayers())
				{
					if (player.getIPAddress().Equals(ip))
					{
						applyToPlayer(task, player);
					}
				}
				break;
			}
			case PunishmentAffect.HWID:
			{
				// String hwid = String.valueOf(task.getKey());
				// for (Player player : World.getInstance().getPlayers())
				// {
				// 	GameClient client = player.getClient();
				// 	if ((client != null) && client.getHardwareInfo().getMacAddress().equals(hwid))
				// 	{
				// 		applyToPlayer(task, player);
				// 	}
				// }
				break;
			}
		}
	}
	
	public void onEnd(PunishmentTask task)
	{
		switch (task.getAffect())
		{
			case PunishmentAffect.CHARACTER:
			{
				int objectId = int.Parse(task.getKey());
				Player player = World.getInstance().getPlayer(objectId);
				if (player != null)
				{
					removeFromPlayer(player);
				}
				break;
			}
			case PunishmentAffect.ACCOUNT:
			{
				String account = task.getKey();
				// GameClient client = LoginServerThread.getInstance().getClient(account);
				// if (client != null)
				// {
				// 	Player player = client.getPlayer();
				// 	if (player != null)
				// 	{
				// 		removeFromPlayer(player);
				// 	}
				// }
				break;
			}
			case PunishmentAffect.IP:
			{
				String ip = task.getKey();
				foreach (Player player in World.getInstance().getPlayers())
				{
					if (player.getIPAddress().Equals(ip))
					{
						removeFromPlayer(player);
					}
				}
				break;
			}
			case PunishmentAffect.HWID:
			{
				String hwid = task.getKey();
				// for (Player player : World.getInstance().getPlayers())
				// {
				// 	GameClient client = player.getClient();
				// 	if ((client != null) && client.getHardwareInfo().getMacAddress().equals(hwid))
				// 	{
				// 		removeFromPlayer(player);
				// 	}
				// }
				break;
			}
		}
	}
	
	/**
	 * Applies all punishment effects from the player.
	 * @param task
	 * @param player
	 */
	private void applyToPlayer(PunishmentTask task, Player player)
	{
		player.setInstance(null);
		
		if (OlympiadManager.getInstance().isRegisteredInComp(player))
		{
			OlympiadManager.getInstance().removeDisconnectedCompetitor(player);
		}
		
		ThreadPool.schedule(new TeleportTask(player, JailZone.getLocationIn()), 2000);
		
		// Open a Html message to inform the player
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/jail_in.htm");
		helper.Replace("%reason%", task != null ? task.getReason() : "");
		helper.Replace("%punishedBy%", task != null ? task.getPunishedBy() : "");

		NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(helper);
		player.sendPacket(msg);
		
		if (task != null)
		{
			TimeSpan? delay = task.getExpirationTime() - DateTime.UtcNow;
			if (delay > TimeSpan.Zero)
			{
				player.sendMessage("You've been jailed for " + delay.Value.ToString("g"));
			}
			else
			{
				player.sendMessage("You've been jailed forever.");
			}
		}
	}
	
	/**
	 * Removes any punishment effects from the player.
	 * @param player
	 */
	private void removeFromPlayer(Player player)
	{
		ThreadPool.schedule(new TeleportTask(player, JailZone.getLocationOut()), 2000);
		
		// Open a Html message to inform the player
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/jail_out.htm");
		NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(helper);
		player.sendPacket(msg);
	}
	
	public PunishmentType getType()
	{
		return PunishmentType.JAIL;
	}
}