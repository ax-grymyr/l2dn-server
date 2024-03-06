using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestGoToLobbyPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        CharacterListPacket characterListPacket = new(session.AccountId, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);

        return ValueTask.CompletedTask;
    }
}