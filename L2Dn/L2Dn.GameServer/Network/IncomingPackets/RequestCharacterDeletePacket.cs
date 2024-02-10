using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestCharacterDeletePacket: IIncomingPacket<GameSession>
{
    private int _charSlot;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        CharacterDeleteFailPacket characterDeleteFailPacket = new(CharacterDeleteFailReason.ProhibitCharDeletion);
        connection.Send(ref characterDeleteFailPacket);
        return ValueTask.CompletedTask;
    }
}
