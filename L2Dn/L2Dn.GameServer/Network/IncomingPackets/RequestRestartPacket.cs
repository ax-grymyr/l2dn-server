using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal readonly struct RequestRestartPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;
        session.State = GameSessionState.CharacterScreen;

        RestartResponsePacket restartResponsePacket = new(true);
        connection.Send(ref restartResponsePacket);

        CharacterListPacket characterListPacket = new(session.AccountName, session.PlayKey1, session.Characters,
            session.SelectedCharacter);

        connection.Send(ref characterListPacket);

        return ValueTask.CompletedTask;
    }
}
