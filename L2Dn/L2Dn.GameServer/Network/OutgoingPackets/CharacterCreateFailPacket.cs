using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct CharacterCreateFailPacket(CharacterCreateFailReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x10); // packet code

        writer.WriteInt32((int)reason);
    }
}
