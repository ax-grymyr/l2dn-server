using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExAcceptJoinMPCCPacket: IIncomingPacket<GameSession>
{
    private int _response;

    public void ReadContent(PacketBitReader reader)
    {
        _response = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player requestor = player.getActiveRequester();
        SystemMessagePacket sm;
        if (requestor == null)
            return ValueTask.CompletedTask;

        if (_response == 1)
        {
            bool newCc = false;
            if (!requestor.getParty().isInCommandChannel())
            {
                new CommandChannel(requestor); // Create new CC
                sm = new SystemMessagePacket(SystemMessageId.THE_COMMAND_CHANNEL_HAS_BEEN_FORMED);
                requestor.sendPacket(sm);
                newCc = true;
            }

            requestor.getParty().getCommandChannel().addParty(player.getParty());
            if (!newCc)
            {
                sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_JOINED_THE_COMMAND_CHANNEL);
                player.sendPacket(sm);
            }
        }
        else
        {
            requestor.sendMessage("The player declined to join your Command Channel.");
        }

        player.setActiveRequester(null);
        requestor.onTransactionResponse();

        return ValueTask.CompletedTask;
    }
}