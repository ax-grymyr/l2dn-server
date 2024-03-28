using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDissmissMPCCRoomPacket: IOutgoingPacket
{
    public static readonly ExDissmissMPCCRoomPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DISSMISS_MPCC_ROOM);
    }
}