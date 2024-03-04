using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExIsCharNameCreatable(CharacterNameValidationResult result): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_IS_CHAR_NAME_CREATABLE);
        
        writer.WriteInt32((int)result);
    }
}