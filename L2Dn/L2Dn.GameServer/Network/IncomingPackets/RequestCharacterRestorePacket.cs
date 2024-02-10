using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestCharacterRestorePacket: IIncomingPacket<GameSession>
{
    private int _charSlot;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
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
