using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CharacterCreateFailPacket(CharacterCreateFailReason reason): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CHARACTER_CREATE_FAIL);

        writer.WriteInt32((int)reason);
    }
}