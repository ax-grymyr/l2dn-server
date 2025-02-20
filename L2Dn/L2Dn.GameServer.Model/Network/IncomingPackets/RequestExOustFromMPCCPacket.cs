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

        Player? target = World.getInstance().getPlayer(_name);
        if (target is null)
        {
            player.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
            return ValueTask.CompletedTask;
        }

        if (player.Equals(target))
            return ValueTask.CompletedTask;

        Party? playerParty = player.getParty();
        Party? targetParty = target.getParty();
        if (target.isInParty() && player.isInParty() && playerParty != null && playerParty.isInCommandChannel() &&
            targetParty != null && targetParty.isInCommandChannel() &&
            playerParty.getCommandChannel().getLeader().Equals(player) && playerParty.getCommandChannel()
                .Equals(targetParty.getCommandChannel()))
        {
            targetParty.getCommandChannel().removeParty(targetParty);

            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.YOU_ARE_DISMISSED_FROM_THE_COMMAND_CHANNEL);
            targetParty.broadcastPacket(sm);

            // check if CC has not been canceled
            if (playerParty.isInCommandChannel())
            {
                sm = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_IS_DISMISSED_FROM_THE_COMMAND_CHANNEL);
                sm.Params.addString(targetParty.getLeader().getName());
                playerParty.getCommandChannel().broadcastPacket(sm);
            }
        }
        else
        {
            player.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
        }

        return ValueTask.CompletedTask;
    }
}