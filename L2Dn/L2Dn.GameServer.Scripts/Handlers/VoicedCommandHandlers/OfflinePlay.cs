using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Mobius
 */
public class OfflinePlay: IVoicedCommandHandler
{
	private static readonly string[] VOICED_COMMANDS =
	{
		"offlineplay"
	};

	private static readonly Action<OnPlayerLogin> ON_PLAYER_LOGIN = @event =>
	{
		if (Config.ENABLE_OFFLINE_PLAY_COMMAND && !string.IsNullOrEmpty(Config.OFFLINE_PLAY_LOGIN_MESSAGE))
        {
            @event.getPlayer().sendPacket(new CreatureSayPacket(null, ChatType.ANNOUNCEMENT, "OfflinePlay",
                Config.OFFLINE_PLAY_LOGIN_MESSAGE));
        }
	};

	public OfflinePlay()
    {
        GlobalEvents.Players.Subscribe(this, ON_PLAYER_LOGIN);
	}

	public bool useVoicedCommand(string command, Player player, string target)
	{
		if (command.equals("offlineplay") && Config.ENABLE_OFFLINE_PLAY_COMMAND)
		{
			if (Config.OFFLINE_PLAY_PREMIUM && !player.hasPremiumStatus())
			{
				player.sendPacket(new ExShowScreenMessagePacket("This command is only available to premium players.", 5000));
				player.sendMessage("This command is only available to premium players.");
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}

			if (!player.isAutoPlaying())
			{
				player.sendPacket(new ExShowScreenMessagePacket("You need to enable auto play before exiting.", 5000));
				player.sendMessage("You need to enable auto play before exiting.");
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}

			if (player.isInVehicle() || player.isInsideZone(ZoneId.PEACE))
			{
				player.sendPacket(new ExShowScreenMessagePacket("You may not log out from this location.", 5000));
				player.sendPacket(SystemMessageId.YOU_MAY_NOT_LOG_OUT_FROM_THIS_LOCATION);
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}

			if (player.isRegisteredOnEvent())
			{
				player.sendPacket(new ExShowScreenMessagePacket("Cannot use this command while registered on an event.", 5000));
				player.sendMessage("Cannot use this command while registered on an event.");
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}

			player.addAction(PlayerAction.OFFLINE_PLAY);
			player.sendPacket(new ConfirmDialogPacket("Do you wish to exit and continue auto play?"));
		}

		return true;
	}

	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}