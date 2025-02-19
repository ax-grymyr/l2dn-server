using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.PunishmentHandlers;

/**
 * This class handles chat ban punishment.
 * @author UnAfraid
 */
public class ChatBanHandler: IPunishmentHandler
{
    public void onStart(PunishmentTask task)
    {
        switch (task.getAffect())
        {
            case PunishmentAffect.CHARACTER:
            {
                int objectId = int.Parse(task.getKey());
                Player? player = World.getInstance().getPlayer(objectId);
                if (player != null)
                {
                    applyToPlayer(task, player);
                }

                break;
            }
            case PunishmentAffect.ACCOUNT:
            {
                string account = task.getKey();
                // GameSession client = LoginServerThread.getInstance().getClient(account);
                // if (client != null)
                // {
                // 	Player? player = client.Player;
                // 	if (player != null)
                // 	{
                // 		applyToPlayer(task, player);
                // 	}
                // }
                break;
            }
            case PunishmentAffect.IP:
            {
                string ip = task.getKey();
                foreach (Player player in World.getInstance().getPlayers())
                {
                    string? ipAddress = player.getClient()?.IpAddress.ToString();
                    if (string.Equals(ipAddress, ip))
                    {
                        applyToPlayer(task, player);
                    }
                }

                break;
            }
            case PunishmentAffect.HWID:
            {
                string hwid = task.getKey();
                foreach (Player player in World.getInstance().getPlayers())
                {
                    string? macAddress = player.getClient()?.HardwareInfo?.getMacAddress();
                    if (string.Equals(macAddress, hwid))
                    {
                        applyToPlayer(task, player);
                    }
                }

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
                Player? player = World.getInstance().getPlayer(objectId);
                if (player != null)
                {
                    removeFromPlayer(player);
                }

                break;
            }
            case PunishmentAffect.ACCOUNT:
            {
                string account = task.getKey();
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
                string ip = task.getKey();
                foreach (Player player in World.getInstance().getPlayers())
                {
                    string? ipAddress = player.getClient()?.IpAddress.ToString();
                    if (string.Equals(ipAddress, ip))
                    {
                        removeFromPlayer(player);
                    }
                }

                break;
            }
            case PunishmentAffect.HWID:
            {
                string hwid = task.getKey();
                foreach (Player player in World.getInstance().getPlayers())
                {
                    string? macAddress = player.getClient()?.HardwareInfo?.getMacAddress();
                    if (string.Equals(macAddress, hwid))
                    {
                        removeFromPlayer(player);
                    }
                }

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
        TimeSpan? delay = task.getExpirationTime() - DateTime.UtcNow;
        if (delay > TimeSpan.Zero)
        {
            player.sendMessage("You've been chat banned for " + delay.Value.ToString("g"));
        }
        else
        {
            player.sendMessage("You've been chat banned forever.");
        }

        player.sendPacket(new EtcStatusUpdatePacket(player));
    }

    /**
     * Removes any punishment effects from the player.
     * @param player
     */
    private void removeFromPlayer(Player player)
    {
        player.sendMessage("Your Chat ban has been lifted");
        player.sendPacket(new EtcStatusUpdatePacket(player));
    }

    public PunishmentType getType()
    {
        return PunishmentType.CHAT_BAN;
    }
}