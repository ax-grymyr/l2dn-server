using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Mobius
 */
public class Offline: IVoicedCommandHandler
{
	private static readonly string[] VOICED_COMMANDS =
	{
		"offline"
	};
	
	public bool useVoicedCommand(string command, Player player, string target)
	{
		if (command.equals("offline") && Config.ENABLE_OFFLINE_COMMAND && (Config.OFFLINE_TRADE_ENABLE || Config.OFFLINE_CRAFT_ENABLE))
		{
			if (!player.isInStoreMode())
			{
				player.sendPacket(SystemMessageId.PRIVATE_STORE_ALREADY_CLOSED);
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
			
			if (player.isInInstance() || player.isInVehicle() || !player.canLogout())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
			
			player.sendPacket(new ConfirmDialogPacket(SystemMessageId.DO_YOU_WISH_TO_EXIT_THE_GAME));
		}
		
		return true;
	}
	
	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}