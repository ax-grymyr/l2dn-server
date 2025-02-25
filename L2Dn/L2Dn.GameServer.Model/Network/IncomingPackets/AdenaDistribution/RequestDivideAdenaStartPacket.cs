using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.AdenaDistribution;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AdenaDistribution;

public struct RequestDivideAdenaStartPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Party? party = player.getParty();
        if (party == null)
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_DISTRIBUTE_ADENA_IF_YOU_ARE_NOT_A_MEMBER_OF_AN_ALLIANCE_OR_A_COMMAND_CHANNEL);
            return ValueTask.CompletedTask;
        }

        CommandChannel? commandChannel = party.getCommandChannel();
        if (commandChannel != null && !commandChannel.isLeader(player))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_PROCEED_AS_YOU_ARE_NOT_AN_ALLIANCE_LEADER_OR_PARTY_LEADER);
            return ValueTask.CompletedTask;
        }

        if (!party.isLeader(player))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_PROCEED_AS_YOU_ARE_NOT_A_PARTY_LEADER);
            return ValueTask.CompletedTask;
        }

        List<Player> targets = commandChannel != null ? commandChannel.getMembers() : party.getMembers();
        if (player.getAdena() < targets.Count)
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA_2);
            return ValueTask.CompletedTask;
        }

        if (targets.Any(t => t.hasRequest<AdenaDistributionRequest>()))
        {
            // Handle that case ?
            return ValueTask.CompletedTask;
        }

        Item? adenaInstance = player.getInventory().getAdenaInstance();
        if (adenaInstance == null)
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA_2);
            return ValueTask.CompletedTask;
        }

        int adenaObjectId = adenaInstance.ObjectId;
        targets.ForEach(t =>
        {
            t.sendPacket(SystemMessageId.ADENA_DISTRIBUTION_HAS_STARTED);
            t.addRequest(new AdenaDistributionRequest(t, player, targets, adenaObjectId, player.getAdena()));
        });

        player.sendPacket(ExDivideAdenaStartPacket.STATIC_PACKET);

        return ValueTask.CompletedTask;
    }
}