using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NormalCameraPacket: IOutgoingPacket
{
    public static readonly NormalCameraPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NORMAL_CAMERA);
    }
}