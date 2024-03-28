using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Channel Delete user command.
 * @author Chris
 */
public class ChannelDelete: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		93
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}

		if (player.isInParty() && player.getParty().isLeader(player) && player.getParty().isInCommandChannel() &&
		    player.getParty().getCommandChannel().getLeader().Equals(player))
		{
			CommandChannel channel = player.getParty().getCommandChannel();
			channel.broadcastPacket(new SystemMessagePacket(SystemMessageId.THE_COMMAND_CHANNEL_IS_DISBANDED));
			channel.disbandChannel();
			return true;
		}

		return false;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}