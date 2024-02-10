using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct CharacterDeleteFailPacket(CharacterDeleteFailReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x1E); // packet code

        writer.WriteInt32((int)reason);
    }
}
