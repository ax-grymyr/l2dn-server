using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Channel Info user command.
 * @author chris_00
 */
public class ChannelInfo: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		97
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		if ((player.getParty() == null) || (player.getParty().getCommandChannel() == null))
		{
			return false;
		}
		
		CommandChannel channel = player.getParty().getCommandChannel();
		player.sendPacket(new ExMultiPartyCommandChannelInfoPacket(channel));
		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}