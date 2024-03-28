using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.PunishmentHandlers;

/**
 * This class handles ban punishment.
 * @author UnAfraid
 */
public class BanHandler: IPunishmentHandler
{
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
					applyToPlayer(player);
				}
				break;
			}
			case PunishmentAffect.ACCOUNT:
			{
				// TODO: implement
				// string account = task.getKey();
				// GameSession client = LoginServerThread.getInstance().getClient(account);
				// if (client != null)
				// {
				// 	Player? player = client.Player;
				// 	if (player != null)
				// 	{
				// 		applyToPlayer(player);
				// 	}
				// 	else
				// 	{
				// 		Disconnection.of(client).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
				// 	}
				// }
				break;
			}
			case PunishmentAffect.IP:
			{
				String ip = task.getKey();
				foreach (Player player in World.getInstance().getPlayers())
				{
					string? ipAddress = player.getClient()?.IpAddress.ToString();
					if (string.Equals(ipAddress, ip))
					{
						applyToPlayer(player);
					}
				}
				break;
			}
			case PunishmentAffect.HWID:
			{
				String hwid = task.getKey();
				foreach (Player player in World.getInstance().getPlayers())
				{
					string? macAddress = player.getClient()?.HardwareInfo?.getMacAddress();  
					if (string.Equals(macAddress, hwid))
					{
						applyToPlayer(player);
					}
				}
				
				break;
			}
		}
	}
	
	public void onEnd(PunishmentTask task)
	{
		// Should not do anything.
	}
	
	/**
	 * Applies all punishment effects from the player.
	 * @param player
	 */
	private void applyToPlayer(Player player)
	{
		Disconnection.of(player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
	}
	
	public PunishmentType getType()
	{
		return PunishmentType.BAN;
	}
}