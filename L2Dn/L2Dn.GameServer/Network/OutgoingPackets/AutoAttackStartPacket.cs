using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AutoAttackStartPacket(int objectId): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(ServerPacketCode.AUTO_ATTACK_START);
        writer.WriteInt32(objectId);
    }
}