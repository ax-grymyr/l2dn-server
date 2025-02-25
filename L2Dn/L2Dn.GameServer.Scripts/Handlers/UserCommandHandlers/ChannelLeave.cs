using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Channel Leave user command.
 * @author Chris, Zoey76
 */
public class ChannelLeave: IUserCommandHandler
{
    private static readonly int[] COMMAND_IDS = [96];

    public bool useUserCommand(int id, Player player)
    {
        if (id != COMMAND_IDS[0])
        {
            return false;
        }

        Party? party = player.getParty();
        Player? leader = party?.getLeader();
        if (party is null || !party.isLeader(player) || leader == null)
        {
            player.sendPacket(SystemMessageId.ONLY_THE_PARTY_LEADER_CAN_LEAVE_THE_COMMAND_CHANNEL);
            return false;
        }

        CommandChannel? channel = party.getCommandChannel();
        if (party.isInCommandChannel() && channel != null)
        {
            channel.removeParty(party);
            leader.sendPacket(SystemMessageId.YOU_HAVE_LEFT_THE_COMMAND_CHANNEL);

            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_HAS_LEFT_THE_COMMAND_CHANNEL);
            sm.Params.addPcName(leader);
            channel.broadcastPacket(sm);
            return true;
        }

        return false;
    }

    public int[] getUserCommandList()
    {
        return COMMAND_IDS;
    }
}