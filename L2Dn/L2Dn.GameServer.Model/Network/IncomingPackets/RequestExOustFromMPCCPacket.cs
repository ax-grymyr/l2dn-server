using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExOustFromMPCCPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player target = World.getInstance().getPlayer(_name);
        if (target is null)
        {
            player.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
            return ValueTask.CompletedTask;
        }

        if (player.Equals(target))
            return ValueTask.CompletedTask;

        if (target.isInParty() && player.isInParty() && player.getParty().isInCommandChannel() &&
            target.getParty().isInCommandChannel() &&
            player.getParty().getCommandChannel().getLeader().Equals(player) && player.getParty().getCommandChannel()
                .Equals(target.getParty().getCommandChannel()))
        {
            target.getParty().getCommandChannel().removeParty(target.getParty());

            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.YOU_ARE_DISMISSED_FROM_THE_COMMAND_CHANNEL);
            target.getParty().broadcastPacket(sm);

            // check if CC has not been canceled
            if (player.getParty().isInCommandChannel())
            {
                sm = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_IS_DISMISSED_FROM_THE_COMMAND_CHANNEL);
                sm.Params.addString(target.getParty().getLeader().getName());
                player.getParty().getCommandChannel().broadcastPacket(sm);
            }
        }
        else
        {
            player.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
        }

        return ValueTask.CompletedTask;
    }
}