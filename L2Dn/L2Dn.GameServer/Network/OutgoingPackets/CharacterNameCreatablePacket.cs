using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct CharacterNameCreatablePacket(CharacterNameValidationResult result): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0xFE); // packet code
        writer.WriteUInt16(0x10B); // packet ex code

        writer.WriteInt32((int)result);
    }
}
