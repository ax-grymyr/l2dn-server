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
    private static readonly int[] COMMAND_IDS = [93];

    public bool useUserCommand(int id, Player player)
    {
        if (id != COMMAND_IDS[0])
        {
            return false;
        }

        Party? party = player.getParty();
        if (party != null && party.isLeader(player) && party.isInCommandChannel() &&
            party.getCommandChannel().getLeader().Equals(player))
        {
            CommandChannel channel = party.getCommandChannel();
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