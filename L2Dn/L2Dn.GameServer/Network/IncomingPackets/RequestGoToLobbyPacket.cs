using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestGoToLobbyPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;

        CharacterListPacket characterListPacket = new(session.AccountName, session.PlayKey1, session.Characters,
            session.SelectedCharacter);

        connection.Send(ref characterListPacket);

        return ValueTask.CompletedTask;
    }
}
