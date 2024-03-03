using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CharacterCreateSuccessPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CHARACTER_CREATE_SUCCESS); // packet code

        writer.WriteInt32(1);
    }
}